using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Sdk;
using Xunit.v3;

namespace Automation.TestFramework.Dynamic.Runners;

internal class SummaryRunner : XunitTestRunnerBase<SummaryRunnerContext, IXunitTest>
{
    public static SummaryRunner Instance { get; } = new();

    public async ValueTask<RunSummary> Run(
        IXunitTest test,
        IMessageBus messageBus,
        ExplicitOption explicitOption,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource,
        IReadOnlyCollection<IBeforeAfterTestAttribute> beforeAfterTestAttributes,
        object?[] constructorArguments)
    {
        await using var context = new SummaryRunnerContext(
            test,
            messageBus,
            explicitOption,
            aggregator,
            cancellationTokenSource,
            beforeAfterTestAttributes,
            constructorArguments);

        await context.InitializeAsync();

        var discoveryRunSummary = await Run(context);

        return context.StepsRunSummary ?? discoveryRunSummary;
    }

    protected override async ValueTask<TimeSpan> InvokeTest(SummaryRunnerContext ctxt, object? testClassInstance)
    {
        // invoke the summary test as usual, in order to discover the steps and store them on the current test case instance
        var discoveryElapsedTime = await base.InvokeTest(ctxt, testClassInstance);
        var steps = ((ObjectModel.TestCase)ctxt.Test.TestCase).GetSteps();
        if (steps.Count == 0)
        {
            // todo discovery failed
            return discoveryElapsedTime;
        }

        var executionElapsedTime = await InvokeSteps(ctxt, steps);
        
        return discoveryElapsedTime + executionElapsedTime;
    }

    private async ValueTask<TimeSpan> InvokeSteps(SummaryRunnerContext ctxt, IReadOnlyCollection<ObjectModel.Step> steps)
    {
        var executionElapsedTime = await ExecutionTimer.MeasureAsync(async () =>
        {
            RunSummary runSummary = new();
            string? skipReason = null;
            foreach (var step in steps)
            {
                var test = step.ToXunitTest();
                if (skipReason is not null) test.SkipReason = skipReason;

                var testRunSummary = await StepRunner.Instance.Run(step, test, ctxt.MessageBus, ctxt.Aggregator, ctxt.CancellationTokenSource);
                runSummary.Aggregate(testRunSummary);

                if (testRunSummary.Failed > 0 && skipReason is null)
                {
                    skipReason = "Skipped because of errors in previous steps";
                }
            }

            ctxt.StepsRunSummary = runSummary;
        });
        
        return executionElapsedTime;
    }
}