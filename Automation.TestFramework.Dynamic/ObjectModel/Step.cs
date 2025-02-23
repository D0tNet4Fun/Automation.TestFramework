using System;
using System.Collections.Generic;
using Automation.TestFramework.Dynamic.Runners;
using Xunit.Internal;

namespace Automation.TestFramework.Dynamic.ObjectModel;

internal class Step(StepType type, int index, int order, string description, Delegate code, IStepDescriptor descriptor)
    : IDynamicStep
{
    public static Step Current => (Step)TestFramework.Step.Current;

    public StepType Type { get; } = type;
    public int Index { get; } = index;
    public int Order { get; } = order;
    public string Description { get; } = Guard.ArgumentNotNull(description, nameof(description));
    public Delegate Code { get; } = Guard.ArgumentNotNull(code, nameof(code));
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
            var testDisplayName = $"[{Index}/{testCase.StepCount}] [{typeDescription}] {Order}. {Description}";

            return testDisplayName;
        }
    }

    public void PreInvoke() => TestFramework.Step.SetCurrent(this);

    public void PostInvoke() => TestFramework.Step.ResetCurrent();
    
    public IStepDescriptor Descriptor => descriptor;

    public TStepDescriptor GetDescriptor<TStepDescriptor>() where TStepDescriptor : IStepDescriptor
    {
        if (descriptor is not TStepDescriptor specificDescriptor)
            throw new InvalidOperationException($"{Type} step descriptor mismatch. Expected: {typeof(TStepDescriptor).Name}, actual: {descriptor.GetType().Name}.");

        return specificDescriptor;
    }

    public IReadOnlyCollection<SubStep> GetSubSteps()
    {
        return ((StepDescriptor)descriptor).GetSubSteps();
    }
    
    public StepRunnerContext? RunnerContext { get; set; }
}