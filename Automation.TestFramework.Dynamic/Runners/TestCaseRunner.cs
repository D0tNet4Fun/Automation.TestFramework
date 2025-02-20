using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Sdk;
using Xunit.v3;

namespace Automation.TestFramework.Dynamic.Runners;

internal class TestCaseRunner : XunitTestCaseRunner
{
    public new static TestCaseRunner Instance { get; } = new();

    public async Task<RunSummary> Run(
        IXunitTestCase testCase,
        IReadOnlyCollection<IXunitTest> tests,
        ExplicitOption explicitOption,
        IMessageBus messageBus,
        object?[] constructorArguments,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource)
    {
        if (tests.Count != 1)
        {
            throw new ArgumentException("The test case should have a single test, the one known as the summary.");
        }

        await using var context = new XunitTestCaseRunnerContext(
            testCase,
            tests,
            messageBus,
            aggregator,
            cancellationTokenSource,
            testCase.TestCaseDisplayName,
            testCase.SkipReason,
            explicitOption,
            constructorArguments);

        await context.InitializeAsync();

        return await Run(context);
    }

    protected override ValueTask<RunSummary> RunTest(XunitTestCaseRunnerContext ctxt, IXunitTest test)
    {
        return SummaryRunner.Instance.Run(
            test,
            ctxt.MessageBus, 
            ctxt.ExplicitOption, 
            ctxt.Aggregator, 
            ctxt.CancellationTokenSource, 
            ctxt.BeforeAfterTestAttributes, 
            ctxt.ConstructorArguments);
    }
}