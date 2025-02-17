using System.Collections.Generic;
using System.Threading;
using Automation.TestFramework.Dynamic.Entities;
using Xunit.Sdk;
using Xunit.v3;

namespace Automation.TestFramework.Dynamic.Execution;

internal class TestCaseRunnerContext(
    TestCase testCase,
    IReadOnlyCollection<IXunitTest> tests,
    IMessageBus messageBus,
    ExceptionAggregator aggregator,
    CancellationTokenSource cancellationTokenSource,
    ExplicitOption explicitOption,
    object?[] constructorArguments)
    : XunitTestCaseRunnerBaseContext<TestCase, IXunitTest>(testCase, tests, messageBus, aggregator, cancellationTokenSource, testCase.TestCaseDisplayName, testCase.SkipReason, explicitOption, constructorArguments)
{
}