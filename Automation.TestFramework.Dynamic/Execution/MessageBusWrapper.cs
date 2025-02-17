using Xunit.Sdk;
using Xunit.v3;

namespace Automation.TestFramework.Dynamic.Execution;

internal class MessageBusWrapper(IMessageBus messageBus) : IMessageBus
{
    public void Dispose() => 
        messageBus.Dispose();

    public bool QueueMessage(IMessageSinkMessage message) => 
        messageBus.QueueMessage(message);
}