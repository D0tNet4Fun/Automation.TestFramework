using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Automation.TestFramework.Dynamic.Entities;
using Xunit.v3;

namespace Automation.TestFramework.Dynamic.Execution;

internal class DynamicTestSetRunner
{
    public static DynamicTestSetRunner Instance { get; } = new();

    public async ValueTask<RunSummary> Run(
        IReadOnlyCollection<IDynamicTest> tests,
        IMessageBus messageBus,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource)
    {
        RunSummary testSetRunSummary = new();
        string? skipReason = null;

        foreach (var test in tests)
        {
            if (skipReason is not null) test.SkipReason = skipReason;
            
            var testRunSummary = await DynamicTestRunner.Instance.Run(test, messageBus, aggregator, cancellationTokenSource);
            testSetRunSummary.Aggregate(testRunSummary);
            
            if (testRunSummary.Failed > 0 && skipReason is null)
            {
                skipReason = "Skipped because of errors in previous steps";
            }
        }

        return testSetRunSummary;
    }
}