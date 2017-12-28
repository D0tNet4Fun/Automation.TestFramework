using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Automation.TestFramework.Entities;
using Xunit.Abstractions;
using Xunit.Sdk;
using ITest = Automation.TestFramework.Entities.ITest;

namespace Automation.TestFramework.Execution
{
    internal class TestCaseRunner : TestCaseRunner<IXunitTestCase>
    {
        private readonly Dictionary<Type, object> _classFixtureMappings;
        private Test _test; // the test bound to the test case
        private TestCaseDefinition _testCaseDefinition;

        public TestCaseRunner(IXunitTestCase testCase, string displayName, string skipReason, object[] constructorArguments, IMessageBus messageBus, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource, Dictionary<Type, object> classFixtureMappings)
            : base(testCase, messageBus, aggregator, cancellationTokenSource)
        {
            DisplayName = displayName;
            SkipReason = skipReason;
            ConstructorArguments = constructorArguments;
            _classFixtureMappings = classFixtureMappings;
            TestClass = TestCase.TestMethod.TestClass.Class.ToRuntimeType();
            TestMethod = TestCase.Method.ToRuntimeMethod();
        }

        public string DisplayName { get; }
        public string SkipReason { get; }
        public object[] ConstructorArguments { get; }

        public Type TestClass { get; }
        public MethodInfo TestMethod { get; }

        protected override async Task AfterTestCaseStartingAsync()
        {
            await base.AfterTestCaseStartingAsync();

            // create the test class instance
            _test = new Test(TestCase, null, TestCase.Method, DisplayName);
            var timer = new ExecutionTimer();
            Aggregator.Run(() =>
            {
                var testClassInstance = _test.CreateTestClass(TestClass, ConstructorArguments, MessageBus, timer, CancellationTokenSource);
                _test.TestClassInstance = testClassInstance;
            });

            // discover the other tests
            Aggregator.Run(() =>
            {
                _testCaseDefinition = new TestCaseDefinition(TestCase, _test.TestClassInstance, _classFixtureMappings);
                _testCaseDefinition.DiscoverTestCaseComponents();
            });
        }

        protected override Task BeforeTestCaseFinishedAsync()
        {
            var timer = new ExecutionTimer();
            Aggregator.Run(() => _test.DisposeTestClass(_test.TestClassInstance, MessageBus, timer, CancellationTokenSource));

            return base.BeforeTestCaseFinishedAsync();
        }

        protected override async Task<RunSummary> RunTestAsync()
        {
            var runSummary = new RunSummary();

            runSummary.Aggregate(await RunTestCaseComponents());

            // run the summary last
            Exception exception = null;
            if (runSummary.Failed > 0)
            {
                exception = new TestCaseFailedException("The test case steps were not completed successfully.");
            }
            var runner = CreateTestRunner(_test, TestCase.Method, exception: exception);
            runSummary.Aggregate(await runner.RunAsync());

            return runSummary;
        }

        private TestRunner CreateTestRunner(ITest test, IMethodInfo testMethod, string skipReason = null, Exception exception = null)
        {
            var method = (testMethod as IReflectionMethodInfo).MethodInfo;
            return new TestRunner(test, MessageBus, TestClass, method, skipReason, exception, new ExceptionAggregator(Aggregator), CancellationTokenSource);
        }

        private async Task<RunSummary> RunTestCaseComponents()
        {
            var runSummary = new RunSummary();
            var skip = false;
            const string skipReason = "An error occurred in a previous step.";

            foreach (var setup in _testCaseDefinition.Setups)
            {
                var runner = CreateTestRunner(setup, setup.MethodInfo, skip ? skipReason : string.Empty);
                runSummary.Aggregate(await runner.RunAsync());
                skip = runSummary.Failed > 0;
            }

            foreach (var precondition in _testCaseDefinition.Preconditions)
            {
                var runner = CreateTestRunner(precondition, precondition.MethodInfo, skip ? skipReason : string.Empty);
                runSummary.Aggregate(await runner.RunAsync());
                skip = runSummary.Failed > 0;
            }

            foreach (var testStep in _testCaseDefinition.Steps)
            {
                var runner = CreateTestRunner(testStep.Input, testStep.Input.MethodInfo, skip ? skipReason : string.Empty);
                runSummary.Aggregate(await runner.RunAsync());
                skip = runSummary.Failed > 0;

                if (testStep.ExpectedResult != null)
                {
                    runner = CreateTestRunner(testStep.ExpectedResult, testStep.ExpectedResult.MethodInfo, skip ? skipReason : string.Empty);
                    runSummary.Aggregate(await runner.RunAsync());
                    skip = runSummary.Failed > 0;
                }
            }

            foreach (var cleanup in _testCaseDefinition.Cleanups)
            {
                var runner = CreateTestRunner(cleanup, cleanup.MethodInfo);
                runSummary.Aggregate(await runner.RunAsync());
            }

            return runSummary;
        }
    }
}