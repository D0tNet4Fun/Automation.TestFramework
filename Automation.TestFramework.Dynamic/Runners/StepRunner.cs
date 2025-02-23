using System;
using System.Threading;
using System.Threading.Tasks;
using Automation.TestFramework.Dynamic.ObjectModel;
using Xunit.v3;

namespace Automation.TestFramework.Dynamic.Runners;

internal class StepRunner : DynamicTestRunnerBase<StepRunnerContext>
{
    public static StepRunner Instance { get; } = new();

    public async ValueTask<RunSummary> Run(
        ObjectModel.Step step,
        IDynamicTest test, 
        IMessageBus messageBus, 
        ExceptionAggregator aggregator, 
        CancellationTokenSource cancellationTokenSource)
    {
        await using var context = new StepRunnerContext(
            step,
            test,
            messageBus,
            aggregator,
            cancellationTokenSource);

        await context.InitializeAsync();

        var runSummary = await Run(context);

        if (context.SubStepsRunSummary is not null)
        {
            runSummary.Aggregate(context.SubStepsRunSummary.Value);
        }

        return runSummary;
    }

    public async ValueTask RunCurrentStepSubSteps()
    {
        var step = ObjectModel.Step.Current;
        var runnerContext = step.RunnerContext ?? throw new InvalidOperationException("The current step does not have a runner context.");
        var subStepsRunSummary = runnerContext.SubStepsRunSummary ?? new RunSummary();

        var subSteps = step.GetSubSteps();
        foreach (var subStep in subSteps)
        {
            var dynamicTest = subStep.ToXunitTest();
            if (runnerContext.HasErrors)
            {
                dynamicTest.SkipReason = "Skipped because errors occurred in previous sub-steps.";
            }

            var subStepRunSummary = await DynamicTestRunner.Instance.Run(dynamicTest, runnerContext.MessageBus, runnerContext.Aggregator, runnerContext.CancellationTokenSource);
            subStepsRunSummary.Aggregate(subStepRunSummary);
            runnerContext.HasErrors = subStepsRunSummary.Failed > 0;
        }

        runnerContext.SubStepsRunSummary = subStepsRunSummary;
    }

    protected override void PreInvoke(StepRunnerContext ctxt)
    {
        ctxt.Step.RunnerContext = ctxt;
        ctxt.Step.PreInvoke();
        base.PreInvoke(ctxt);
    }

    protected override async ValueTask<TimeSpan> InvokeTest(StepRunnerContext ctxt, object? testClassInstance)
    {
        var elapsed =  await base.InvokeTest(ctxt, testClassInstance);

        if (ctxt.SubStepsRunSummary is not null)
        {
            var runSummary = ctxt.SubStepsRunSummary.Value;
            var hasErrors = runSummary.Failed > 0 || runSummary.Skipped > 0 || runSummary.NotRun > 0;
            if (hasErrors)
            {
                throw new StepExecutionException("One or more errors occurred while running this step.");
            }
        }

        return elapsed;
    }

    protected override void PostInvoke(StepRunnerContext ctxt)
    {
        try
        {
            base.PostInvoke(ctxt);
        }
        finally
        {
            ctxt.Step.PostInvoke();
            ctxt.Step.RunnerContext = null;
        }
    }
}