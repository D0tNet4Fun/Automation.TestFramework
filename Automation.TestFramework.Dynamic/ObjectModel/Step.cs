using System;
using System.Collections.Generic;
using Automation.TestFramework.Dynamic.Runners;
using Xunit.Sdk;

namespace Automation.TestFramework.Dynamic.ObjectModel;

internal class Step : IStep
{
    private readonly Queue<SubStep> _subSteps = [];

    public Step(StepType type, int index, int order, string description, Delegate code)
    {
        if (string.IsNullOrEmpty(description)) throw new ArgumentNullException(nameof(description));

        Type = type;
        Index = index;
        Order = order;
        Description = description;
        Code = code;
    }

    public static Step Current => (Step)Automation.TestFramework.Step.Current;

    public StepType Type { get; }
    public int Index { get; set; }
    public int Order { get; }
    public string Description { get; }
    public Delegate Code { get; }
    public int? Timeout { get; set; }
    public int SubStepCount => _subSteps.Count;

    public IDynamicTest ToXunitTest()
    {
        var testCase = TestCase.Current;
        var displayName = GetTestDisplayName();

        return new DynamicTest(
            testCase,
            displayName,
            uniqueID: UniqueIDGenerator.ForTest(testCase.UniqueID, testCase.GetNextTestIndex()),
            Timeout,
            Code);

        string GetTestDisplayName()
        {
            // sample: [1/10] [Input] 1. Do something
            var typeDescription = Type.GetDisplayName();
            var testDisplayName = $"[{Index}/{testCase.StepCount}] [{typeDescription}] {Order}. {Description}";

            return testDisplayName;
        }
    }

    public IStep ExecuteSubStep(SubStepType type, string description, Action action)
    {
        AddSubStep(type, description, action);
        Execute();

        return this;
    }

    private Step AddSubStep(SubStepType type, string description, Delegate code)
    {
        var subStepIndex = _subSteps.Count + 1;
        var subStep = new SubStep(type, subStepIndex , description, code);
        _subSteps.Enqueue(subStep);

        return this;
    }

    private void Execute()
    {
        var subStep = _subSteps.Dequeue();
        var (messageBus, aggregator, cancellationTokenSource) = TestCase.Current.RuntimeDependencies!;

        var dynamicTest = subStep.ToXunitTest();
        var task = StepRunner.Instance.Run(this, dynamicTest, messageBus, aggregator, cancellationTokenSource);
        task.GetAwaiter().GetResult();
    }

    public void PreInvoke()
    {
        TestFramework.Step.SetCurrent(this);
    }

    public void PostInvoke()
    {
        TestFramework.Step.ResetCurrent();
    }
}