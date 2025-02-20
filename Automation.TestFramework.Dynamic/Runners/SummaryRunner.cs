using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Internal;
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
        var steps = ((TestCase)TestCase.Current).GetSteps();
        if (steps.Count == 0)
        {
            // todo discovery failed
            return discoveryElapsedTime;
        }
        
        // create the dynamic tests from steps
        var tests = steps
            .Select((step, index) => step.ToXunitTest(ctxt.Test.TestCase, index + 1, steps.Count))
            .CastOrToReadOnlyList();

        // execute the dynamic tests and cache their summary
        var (stepsRunSummary, executionElapsedTime) = await ExecutionTimer.MeasureAsync(() =>
            DynamicTestSetRunner.Instance.Run(tests, ctxt.MessageBus, ctxt.Aggregator, ctxt.CancellationTokenSource));
        ctxt.StepsRunSummary = stepsRunSummary;
        
        return discoveryElapsedTime + executionElapsedTime;
    }
}