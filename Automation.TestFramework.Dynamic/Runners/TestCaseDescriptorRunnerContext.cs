using System.Collections.Generic;
using System.Threading;
using Automation.TestFramework.Dynamic.ObjectModel;
using Xunit.Sdk;
using Xunit.v3;
using ITestCase = Automation.TestFramework.Dynamic.ObjectModel.ITestCase;

namespace Automation.TestFramework.Dynamic.Runners;

internal class TestCaseDescriptorRunnerContext(
    IXunitTest test,
    ITestCase testCase,
    IMessageBus messageBus,
    ExplicitOption explicitOption,
    ExceptionAggregator aggregator,
    CancellationTokenSource cancellationTokenSource,
    IReadOnlyCollection<IBeforeAfterTestAttribute> beforeAfterTestAttributes,
    object?[] constructorArguments)
    : XunitTestRunnerContext(test, messageBus, explicitOption, aggregator, cancellationTokenSource, beforeAfterTestAttributes, constructorArguments)
{
    public ITestCase TestCase { get; } = testCase;
    public RunSummary? StepsRunSummary { get; set; }
}