using System.Collections.Generic;
using System.Threading;
using Xunit.Sdk;
using Xunit.v3;

namespace Automation.TestFramework.Dynamic.Runners;

internal class SummaryRunnerContext(
    IXunitTest test,
    IMessageBus messageBus,
    ExplicitOption explicitOption,
    ExceptionAggregator aggregator,
    CancellationTokenSource cancellationTokenSource,
    IReadOnlyCollection<IBeforeAfterTestAttribute> beforeAfterTestAttributes,
    object?[] constructorArguments)
    : XunitTestRunnerContext(test, messageBus, explicitOption, aggregator, cancellationTokenSource, beforeAfterTestAttributes, constructorArguments)
{
    public RunSummary? StepsRunSummary { get; set; }
}