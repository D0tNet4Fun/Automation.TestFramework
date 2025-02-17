using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Automation.TestFramework.Dynamic.Entities;
using Xunit.Sdk;
using Xunit.v3;

namespace Automation.TestFramework.Dynamic.Execution;

internal class TestCaseRunner : XunitTestCaseRunnerBase<TestCaseRunnerContext, TestCase, IXunitTest>
{
    public static TestCaseRunner Instance { get; } = new();

    public async Task<RunSummary> Run(
        TestCase testCase,
        IReadOnlyCollection<IXunitTest> tests,
        ExplicitOption explicitOption,
        IMessageBus messageBus,
        object?[] constructorArguments,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource)
    {
        if (tests.Count != 1)
        {
            throw new ArgumentException("The test case should have a single static test, the one known as the summary.");
        }
        
        await using var context = new TestCaseRunnerContext(
            testCase,
            tests,
            messageBus,
            aggregator,
            cancellationTokenSource,
            explicitOption,
            constructorArguments);

        await context.InitializeAsync();

        return await Run(context);
    }

    protected override ValueTask<RunSummary> RunTest(TestCaseRunnerContext ctxt, IXunitTest test)
    {
        return SummaryRunner.Instance.Run(ctxt, test);
    }
}