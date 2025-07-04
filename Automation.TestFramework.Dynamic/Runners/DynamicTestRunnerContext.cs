using System.Threading;
using Automation.TestFramework.Dynamic.ObjectModel;
using Xunit.Sdk;
using Xunit.v3;

namespace Automation.TestFramework.Dynamic.Runners;

internal class DynamicTestRunnerContext(
    IDynamicTest test,
    IMessageBus messageBus,
    ExceptionAggregator aggregator,
    CancellationTokenSource cancellationTokenSource)
    : XunitTestRunnerBaseContext<IDynamicTest>(
        test, messageBus, ExplicitOption.Off, aggregator, cancellationTokenSource,
        beforeAfterTestAttributes: [], // not needed 
        constructorArguments: [] // not needed
    );