using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Automation.TestFramework.Dynamic.Runners;
using Xunit.v3;

namespace Automation.TestFramework.Dynamic.ObjectModel;

internal class StepDescriptor : IStepDescriptor
{
    private readonly Queue<SubStep> _subSteps = [];

    public int SubStepCount { get; private set; }

    public IStepDescriptor ExecuteSubStep(SubStepType type, string description, Action code)
    {
        AddSubStepFromDelegate(type, description, code);
        Execute();

        return this;
    }

    public IStepDescriptor ExecuteAsyncSubStep(SubStepType type, string description, Func<Task> code)
    {
        AddSubStepFromDelegate(type, description, code);
        Execute();

        return this;
    }

    private IStepDescriptor AddSubStepFromDelegate(SubStepType type, string description, Delegate code)
    {
        var order = SubStepCount + 1;
        var subStep = new SubStep(type, order, description, code);
        _subSteps.Enqueue(subStep);
        SubStepCount++;

        return this;
    }

    private void Execute()
    {
        var runnerContext = Step.Current.RunnerContext ?? throw new InvalidOperationException("The current step does not have a runner context.");
        var subStepsRunSummary = runnerContext.SubStepsRunSummary ?? new RunSummary();

        while (_subSteps.Count > 0)
        {
            var subStep = _subSteps.Dequeue();
            var dynamicTest = subStep.ToXunitTest();

            var task = DynamicTestRunner.Instance.Run(dynamicTest, runnerContext.MessageBus, runnerContext.Aggregator, runnerContext.CancellationTokenSource);
            var subStepRunSummary = task.GetAwaiter().GetResult();
            
            subStepsRunSummary.Aggregate(subStepRunSummary);
        }

        runnerContext.SubStepsRunSummary = subStepsRunSummary;
    }
}