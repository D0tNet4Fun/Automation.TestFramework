using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Automation.TestFramework.Dynamic.ObjectModel;
using Xunit.Sdk;
using Xunit.v3;
using ITestCase = Automation.TestFramework.Dynamic.ObjectModel.ITestCase;

namespace Automation.TestFramework.Dynamic.Runners;

internal class TestCaseDescriptorRunner : XunitTestRunnerBase<TestCaseDescriptorRunnerContext, IXunitTest>
{
    public static TestCaseDescriptorRunner Instance { get; } = new();

    public async ValueTask<RunSummary> Run(
        IXunitTest test,
        ITestCase testCase,
        IMessageBus messageBus,
        ExplicitOption explicitOption,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource,
        IReadOnlyCollection<IBeforeAfterTestAttribute> beforeAfterTestAttributes,
        object?[] constructorArguments)
    {
        await using var context = new TestCaseDescriptorRunnerContext(
            test,
            testCase,
            messageBus,
            explicitOption,
            aggregator,
            cancellationTokenSource,
            beforeAfterTestAttributes,
            constructorArguments);

        await context.InitializeAsync();

        var discoveryRunSummary = await Run(context);

        return context.StepsRunSummary ?? discoveryRunSummary;
    }

    protected override async ValueTask<TimeSpan> InvokeTest(TestCaseDescriptorRunnerContext ctxt, object? testClassInstance)
    {
        // invoke the summary test as usual, in order to discover the steps and store them on the current test case instance
        var discoveryElapsedTime = await base.InvokeTest(ctxt, testClassInstance);
        var steps = ctxt.TestCase.GetSteps();
        if (steps.Count == 0)
        {
            // todo discovery failed
            return discoveryElapsedTime;
        }

        var executionElapsedTime = await InvokeSteps(ctxt, steps);
        
        return discoveryElapsedTime + executionElapsedTime;
    }

    private async ValueTask<TimeSpan> InvokeSteps(TestCaseDescriptorRunnerContext ctxt, IReadOnlyCollection<ObjectModel.Step> steps)
    {
        var executionElapsedTime = await ExecutionTimer.MeasureAsync(async () =>
        {
            RuntimeDependencies runtimeDependencies = new(ctxt.MessageBus, ctxt.Aggregator, ctxt.CancellationTokenSource);
            
            RunSummary runSummary = new();
            string? skipReason = null;
            foreach (var step in steps)
            {
                step.RuntimeDependencies = runtimeDependencies;
                
                var test = step.ToXunitTest();
                if (skipReason is not null) test.SkipReason = skipReason;

                var testRunSummary = await StepRunner.Instance.Run(step, test, ctxt.MessageBus, ctxt.Aggregator, ctxt.CancellationTokenSource);
                runSummary.Aggregate(testRunSummary);

                if (testRunSummary.Failed > 0 && skipReason is null)
                {
                    skipReason = "Skipped because of errors in previous steps";
                }
            }

            ctxt.StepsRunSummary = runSummary;
        });
        
        return executionElapsedTime;
    }
}