using System;
using System.Collections.Generic;
using Automation.TestFramework.Dynamic.Runners;
using Xunit.Internal;

namespace Automation.TestFramework.Dynamic.ObjectModel;

internal class Step : IDynamicStep
{
    private readonly StepDescriptor _descriptor;
    
    public Step(StepType type, int index, int order, string description, Delegate code, Func<Step, StepDescriptor> descriptorFactory)
    {
        Type = type;
        Index = index;
        Order = order;
        Description = Guard.ArgumentNotNull(description, nameof(description));
        Code = Guard.ArgumentNotNull(code, nameof(code));
        _descriptor = descriptorFactory(this); 
    }

    public static Step Current => (Step)TestFramework.Step.Current;

    public StepType Type { get; }
    public int Index { get; }
    public int Order { get; }
    public string Description { get; }
    public Delegate Code { get; }
    public int? Timeout { get; set; }

    public IDynamicTest ToXunitTest()
    {
        var testCase = TestCase.Current;
        var displayName = GetTestDisplayName();

        return new DynamicTest(
            testCase,
            displayName,
            uniqueId: testCase.GetNextDynamicTestUniqueId(),
            Timeout,
            Code);

        string GetTestDisplayName()
        {
            // sample: [1/10] [Input] 1. Do something
            var typeDescription = Type.GetDisplayName();
            var testDisplayName = $"[{GetPosition(testCase.StepCount)}] [{typeDescription}] {Order}. {Description}";

            return testDisplayName;
        }
    }

    public string GetPosition(int count)
    {
        // samples: [1/9], [01/10], [001/100]
        var maxDigits = count.ToString().Length;

        return $"{Index.ToString().PadLeft(maxDigits, '0')}/{count}";
    }

    public void PreInvoke() => TestFramework.Step.SetCurrent(this);

    public void PostInvoke() => TestFramework.Step.ResetCurrent();

    public IStepDescriptor Descriptor => _descriptor;

    public TStepDescriptor GetDescriptor<TStepDescriptor>() where TStepDescriptor : IStepDescriptor
    {
        if (_descriptor is not TStepDescriptor specificDescriptor)
            throw new InvalidOperationException($"{Type} step descriptor mismatch. Expected: {typeof(TStepDescriptor).Name}, actual: {_descriptor.GetType().Name}.");

        return specificDescriptor;
    }

    public StepRunnerContext? RunnerContext { get; set; }

    public bool HasPendingSubSteps => _descriptor.HasPendingSubSteps;

    public void Execute(IReadOnlyCollection<SubStep> subSteps)
    {
        var task = SubStepRunner.Instance.Run(
            RunnerContext,
            subSteps);
        task.GetAwaiter().GetResult();
    }
}