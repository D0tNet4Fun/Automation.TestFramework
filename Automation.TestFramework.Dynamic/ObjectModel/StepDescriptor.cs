using System;
using System.Collections.Generic;
using Automation.TestFramework.Dynamic.Runners;
using Xunit.v3;

namespace Automation.TestFramework.Dynamic.ObjectModel;

internal class StepDescriptor : IStepDescriptor
{
    private readonly Queue<SubStep> _subSteps = [];

    public IStepDescriptor ExecuteSubStep(SubStepType type, string description, Action code)
    {
        AddSubStep(type, description, code);
        Execute();

        return this;
    }

    private IStepDescriptor AddSubStep(SubStepType type, string description, Delegate code)
    {
        var order = _subSteps.Count + 1;
        var subStep = new SubStep(type, order , description, code);
        _subSteps.Enqueue(subStep);

        return this;
    }

    private void Execute()
    {
        var (messageBus, aggregator, cancellationTokenSource) =  Step.Current.RuntimeDependencies
                                                                ?? throw new InvalidOperationException("Runtime dependencies are not available.");

        while (_subSteps.Count > 0)
        {
            var subStep = _subSteps.Dequeue();
            var dynamicTest = subStep.ToXunitTest();

            ExecutionTimer.Measure(async () => await DynamicTestRunner.Instance.Run(dynamicTest, messageBus, aggregator, cancellationTokenSource));
        }
    }
}