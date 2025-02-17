using System;
using System.Linq;
using System.Threading.Tasks;
using Automation.TestFramework.Dynamic.Entities;
using Xunit.v3;

namespace Automation.TestFramework.Dynamic.Execution;

internal class SummaryRunner : XunitTestRunnerBase<SummaryRunnerContext, IXunitTest>
{
    public static SummaryRunner Instance { get; } = new();

    public async ValueTask<RunSummary> Run(
        TestCaseRunnerContext testCaseCtx,
        IXunitTest test)
    {
        await using var context = new SummaryRunnerContext(
            test,
            testCaseCtx);

        await context.InitializeAsync();

        var discoveryRunSummary = await Run(context);

        return context.StepsRunSummary ?? discoveryRunSummary;
    }

    protected override async ValueTask<TimeSpan> InvokeTest(SummaryRunnerContext ctxt, object? testClassInstance)
    {
        // invoke the summary test as usual, in order to collect the dynamic steps on the current test case instance
        var discoveryElapsedTime = await base.InvokeTest(ctxt, testClassInstance);

        // create the tests from steps
        var tests = ctxt.TestCaseCtx.TestCase.CreateTestsFromSteps();
        if (tests.Count == 0)
        {
            // todo discovery failed
            return discoveryElapsedTime;
        }

        // execute the dynamic steps and cache their summary
        var (stepsRunSummary, executionElapsedTime) = await ExecutionTimer.MeasureAsync(() =>
            DynamicTestSetRunner.Instance.Run(tests, ctxt.MessageBus, ctxt.Aggregator, ctxt.CancellationTokenSource));
        ctxt.StepsRunSummary = stepsRunSummary;
        
        return discoveryElapsedTime + executionElapsedTime;
    }
}