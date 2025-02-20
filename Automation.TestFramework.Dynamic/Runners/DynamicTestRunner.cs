using System;
using System.Threading;
using System.Threading.Tasks;
using Automation.TestFramework.Dynamic.ObjectModel;
using Xunit.v3;

namespace Automation.TestFramework.Dynamic.Runners;

/// <summary>
/// Runner for dynamic tests.
/// </summary>
internal class DynamicTestRunner : XunitTestRunnerBase<DynamicTestRunnerContext, IDynamicTest>
{
    public static DynamicTestRunner Instance { get; } = new();

    public async ValueTask<RunSummary> Run(
        IDynamicTest test,
        IMessageBus messageBus,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource)
    {
        await using var context = new DynamicTestRunnerContext(
            test,
            messageBus,
            aggregator,
            cancellationTokenSource);

        await context.InitializeAsync();

        return await Run(context);
    }

    protected override bool IsTestClassCreatable(DynamicTestRunnerContext ctxt)
    {
        // dynamic tests are based on delegates. they do not need to create test class instances.   
        return false;
    }

    protected override ValueTask<TimeSpan> InvokeTest(DynamicTestRunnerContext ctxt, object? testClassInstance)
    {
        // invoke the test using the dynamic test's target, instead of the provided test class instance (which is null, because the test runner does not create it)
        var target = ctxt.Test.Target;
        return base.InvokeTest(ctxt, target);
    }

    protected override bool IsTestClassDisposable(DynamicTestRunnerContext ctxt, object testClassInstance)
    {
        // dynamic tests are based on delegates
        return false;
    }
}