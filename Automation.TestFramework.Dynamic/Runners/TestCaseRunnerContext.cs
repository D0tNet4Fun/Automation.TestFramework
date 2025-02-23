using System.Collections.Generic;
using System.Threading;
using Automation.TestFramework.Dynamic.ObjectModel;
using Xunit.Sdk;
using Xunit.v3;
using ITestCase = Automation.TestFramework.Dynamic.ObjectModel.ITestCase;

namespace Automation.TestFramework.Dynamic.Runners;

internal class TestCaseRunnerContext(
    ITestCase testCase,
    IReadOnlyCollection<IXunitTest> tests,
    IMessageBus messageBus,
    ExplicitOption explicitOption,
    ExceptionAggregator aggregator,
    CancellationTokenSource cancellationTokenSource,
    object?[] constructorArguments)
    : XunitTestCaseRunnerBaseContext<ITestCase, IXunitTest>(
        testCase,
        tests,
        messageBus,
        aggregator,
        cancellationTokenSource,
        testCase.TestCaseDisplayName,
        testCase.SkipReason,
        explicitOption,
        constructorArguments);