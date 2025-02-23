using System;
using System.Threading;
using System.Threading.Tasks;
using Automation.TestFramework.Dynamic.Framework;
using Automation.TestFramework.Dynamic.ObjectModel;
using Xunit;
using Xunit.v3;

namespace Automation.TestFramework.Dynamic.Runners;

internal class StepRunner : DynamicTestRunnerBase<StepRunnerContext>
{
    public static StepRunner Instance { get; } = new();

    public async ValueTask<RunSummary> Run(
        ObjectModel.Step step,
        IDynamicTest test,
        object? testClassInstance,
        IMessageBus messageBus,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource)
    {
        await using var context = new StepRunnerContext(
            step,
            test,
            testClassInstance,
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
            // skip this test if there are critical errors from running previous sub-steps
            if (runnerContext.HasCriticalErrors)
            {
                dynamicTest.SkipReason = "Skipped because errors occurred in previous sub-steps.";
            }

            var subStepRunSummary = await DynamicTestRunner.Instance.Run(dynamicTest, runnerContext.MessageBus, runnerContext.Aggregator, runnerContext.CancellationTokenSource);
            subStepsRunSummary.Aggregate(subStepRunSummary);

            // acknowledge there are critical errors while running the current step, if this sub-step failed, and it's not a verification
            // this will cause all future sub-steps to be skipped
            runnerContext.HasCriticalErrors = subStepsRunSummary.Failed > 0 && subStep.Type != SubStepType.Verification;
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
        var elapsed = await base.InvokeTest(ctxt, testClassInstance);

        if (ctxt.Aggregator.HasExceptions)
        {
            TryRaiseErrorEvent(ctxt.TestClassInstance, ctxt.Aggregator.ToException()!);
        }

        if (ctxt.SubStepsRunSummary is not null)
        {
            var runSummary = ctxt.SubStepsRunSummary.Value;
            var hasErrors = runSummary.Failed > 0 || runSummary.Skipped > 0 || runSummary.NotRun > 0;
            if (hasErrors)
            {
                ctxt.Aggregator.Add(new StepFailedException("One or more errors occurred while running this step."));
            }
        }

        return elapsed;
    }

    private static void TryRaiseErrorEvent(object? testClassInstance, Exception exception)
    {
        try
        {
            EventSource.Instance.OnStepError(testClassInstance, exception);
        }
        catch (Exception e)
        {
            // exception thrown by the error handler
            TestContext.Current.AddWarning($"Error in custom event handler: {e.Message}");
        }
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