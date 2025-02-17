using Xunit.v3;

namespace Automation.TestFramework.Dynamic.Execution;

internal class SummaryRunnerContext(
    IXunitTest test,
    TestCaseRunnerContext testCaseCtx) :
    XunitTestRunnerBaseContext<IXunitTest>(test, testCaseCtx.MessageBus, testCaseCtx.ExplicitOption, testCaseCtx.Aggregator, testCaseCtx.CancellationTokenSource, testCaseCtx.BeforeAfterTestAttributes, testCaseCtx.ConstructorArguments)
{
    public TestCaseRunnerContext TestCaseCtx { get; } = testCaseCtx;
    public RunSummary? StepsRunSummary { get; set; }
}