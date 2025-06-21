using System.Threading;
using System.Threading.Tasks;
using Automation.TestFramework.Dynamic.ObjectModel;
using Xunit.v3;

namespace Automation.TestFramework.Dynamic.Runners;

internal class DynamicTestRunner : DynamicTestRunnerBase<DynamicTestRunnerContext>
{
    public static DynamicTestRunner Instance { get; } = new();

    public async Task<RunSummary> Run(
        IDynamicTest test, 
        IMessageBus messageBus, 
        ExceptionAggregator aggregator, 
        CancellationTokenSource cancellationTokenSource)
    {
        await using var context = new DynamicTestRunnerContext(
            test,
            messageBus,
            aggregator,
            cancellationTokenSource);

        await context.InitializeAsync();

        return await Run(context);
    }
}