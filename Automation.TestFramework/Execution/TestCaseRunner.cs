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
        private readonly Type _testNotificationType;
        private Test _test; // the test bound to the test case
        private TestCaseDefinition _testCaseDefinition;
        private Exception _testCaseDiscoveryException;

        public TestCaseRunner(IXunitTestCase testCase, string displayName, string skipReason, object[] constructorArguments, IMessageBus messageBus, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource, Dictionary<Type, object> classFixtureMappings, Type testNotificationType)
            : base(testCase, messageBus, aggregator, cancellationTokenSource)
        {
            DisplayName = displayName;
            SkipReason = skipReason;
            ConstructorArguments = constructorArguments;
            _classFixtureMappings = classFixtureMappings;
            _testNotificationType = testNotificationType;
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

            try
            {
                // create the test class instance
                _test = new Test(TestCase, null, TestCase.Method, DisplayName);
                var timer = new ExecutionTimer();
                var testClassInstance = _test.CreateTestClass(TestClass, ConstructorArguments, MessageBus, timer, CancellationTokenSource);
                _test.TestClassInstance = testClassInstance;

                // discover the other tests
                _testCaseDefinition = new TestCaseDefinition(new TestCaseWithoutTraits(TestCase), _test.TestClassInstance, _classFixtureMappings);
                _testCaseDefinition.DiscoverTestCaseComponents();
            }
            catch (Exception ex)
            {
                _testCaseDiscoveryException = ex;
            }
        }

        protected override Task BeforeTestCaseFinishedAsync()
        {
            var timer = new ExecutionTimer();
            Aggregator.Run(() => _test.DisposeTestClass(_test.TestClassInstance, MessageBus, timer, CancellationTokenSource));

            return base.BeforeTestCaseFinishedAsync();
        }

        protected override async Task<RunSummary> RunTestAsync()
        {
            // if there was an exception during test discovery then handle it here instead of anything else
            if (_testCaseDiscoveryException != null)
                return FailBecauseOfException(_testCaseDiscoveryException);

            var runSummary = new RunSummary();
            // run the test case components first
            runSummary.Aggregate(await RunTestCaseComponents());
            // run the Summary last
            runSummary.Aggregate(await RunTestCaseSummary(runSummary.Failed > 0));
            return runSummary;
        }

        private async Task<RunSummary> RunTestCaseComponents()
        {
            var runSummary = new RunSummary();
            var skip = false;
            const string skipReason = "An error occurred in a previous step."; // we'll show this when skip becomes true

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

        private async Task<RunSummary> RunTestCaseSummary(bool hasErrors)
        {
            if (hasErrors)
                return FailBecauseOfException(new TestCaseFailedException("The test case steps were not completed successfully."));

            var runner = CreateTestRunner(_test, TestCase.Method);
            return await runner.RunAsync();
        }

        private ITestRunner CreateTestRunner(ITest test, IMethodInfo testMethod, string skipReason = null)
        {
            if (test.Actions.Count > 0)
            {
                return new ComplexTestRunner(test,
                    t => new TestRunner(t, MessageBus, t.TestClassInstance.GetType(), ConstructorArguments, t.MethodInfo.ToRuntimeMethod(), skipReason, new ExceptionAggregator(Aggregator), CancellationTokenSource, _testNotificationType));
            }
            return new TestRunner(test, MessageBus, TestClass, ConstructorArguments, testMethod.ToRuntimeMethod(), skipReason, new ExceptionAggregator(Aggregator), CancellationTokenSource, _testNotificationType);
        }

        private RunSummary FailBecauseOfException(Exception exception)
        {
            var test = new XunitTest(new TestCaseWithoutTraits(TestCase), DisplayName);

            if (!MessageBus.QueueMessage(new TestStarting(test)))
                CancellationTokenSource.Cancel();
            else if (!MessageBus.QueueMessage(new TestFailed(test, 0, null, exception)))
                CancellationTokenSource.Cancel();
            if (!MessageBus.QueueMessage(new TestFinished(test, 0, null)))
                CancellationTokenSource.Cancel();

            return new RunSummary { Total = 1, Failed = 1 };
        }
    }
}