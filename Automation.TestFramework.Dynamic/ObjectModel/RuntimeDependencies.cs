using System.Threading;
using Xunit.v3;

namespace Automation.TestFramework.Dynamic.ObjectModel;

internal class RuntimeDependencies(
    IMessageBus messageBus,
    ExceptionAggregator aggregator,
    CancellationTokenSource cancellationTokenSource)
{
    public IMessageBus MessageBus { get; } = messageBus;
    public ExceptionAggregator Aggregator { get; } = aggregator;
    public CancellationTokenSource CancellationTokenSource { get; } = cancellationTokenSource;

    public void Deconstruct(out IMessageBus messageBus, out ExceptionAggregator aggregator, out CancellationTokenSource cancellationTokenSource)
    {
        messageBus = MessageBus;
        aggregator = Aggregator;
        cancellationTokenSource = CancellationTokenSource;
    }
}