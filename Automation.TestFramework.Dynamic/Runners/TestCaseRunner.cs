using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Sdk;
using Xunit.v3;
using ITestCase = Automation.TestFramework.Dynamic.ObjectModel.ITestCase;

namespace Automation.TestFramework.Dynamic.Runners;

internal class TestCaseRunner : XunitTestCaseRunnerBase<TestCaseRunnerContext, ITestCase, IXunitTest>
{
    public static TestCaseRunner Instance { get; } = new();

    public async Task<RunSummary> Run(
        ITestCase testCase,
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

        await using var context = new TestCaseRunnerContext(
            testCase,
            tests,
            messageBus,
            explicitOption,
            aggregator,
            cancellationTokenSource,
            constructorArguments);

        await context.InitializeAsync();

        return await Run(context);
    }

    protected override ValueTask<RunSummary> RunTest(TestCaseRunnerContext ctxt, IXunitTest test)
    {
        return TestCaseDescriptorRunner.Instance.Run(
            test,
            ctxt.TestCase,
            ctxt.MessageBus, 
            ctxt.ExplicitOption, 
            ctxt.Aggregator, 
            ctxt.CancellationTokenSource, 
            ctxt.BeforeAfterTestAttributes, 
            ctxt.ConstructorArguments);
    }
}