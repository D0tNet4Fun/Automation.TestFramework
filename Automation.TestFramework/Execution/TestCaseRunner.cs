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
        private readonly IMessageSink _diagnosticMessageSink;
        private readonly Dictionary<Type, object> _classFixtureMappings;
        private readonly Type _testNotificationType;
        private Test _test; // the test bound to the test case
        private object _testClassInstance;
        private TestCaseDefinition _testCaseDefinition;
        private Exception _testCaseDiscoveryException;

        public TestCaseRunner(IXunitTestCase testCase, string displayName, string skipReason, IMessageSink diagnosticMessageSink, IMessageBus messageBus, object[] constructorArguments, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource, Dictionary<Type, object> classFixtureMappings, Type testNotificationType)
            : base(testCase, new MyMessageBus(messageBus), aggregator, cancellationTokenSource)
        {
            DisplayName = displayName;
            SkipReason = skipReason;
            ConstructorArguments = constructorArguments;
            _diagnosticMessageSink = diagnosticMessageSink;
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
                _testClassInstance = _test.CreateTestClass(TestClass, ConstructorArguments, MessageBus, timer, CancellationTokenSource);

                // discover the other tests
                _testCaseDefinition = new TestCaseDefinition(TestCase, _testClassInstance, _classFixtureMappings, _diagnosticMessageSink);
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
            Aggregator.Run(() => _test.DisposeTestClass(_testClassInstance, MessageBus, timer, CancellationTokenSource));

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
                var runner = CreateTestRunner(setup, setup.MethodInfo, skip ? skipReason : string.Empty, _testNotificationType);
                runSummary.Aggregate(await runner.RunAsync());
                skip = runSummary.Failed > 0;
            }

            foreach (var precondition in _testCaseDefinition.Preconditions)
            {
                var runner = CreateTestRunner(precondition, precondition.MethodInfo, skip ? skipReason : string.Empty, _testNotificationType);
                runSummary.Aggregate(await runner.RunAsync());
                skip = runSummary.Failed > 0;
            }

            foreach (var testStep in _testCaseDefinition.Steps)
            {
                var runner = CreateTestRunner(testStep.Input, testStep.Input.MethodInfo, skip ? skipReason : string.Empty, _testNotificationType);
                runSummary.Aggregate(await runner.RunAsync());
                skip = runSummary.Failed > 0;

                if (testStep.ExpectedResult != null)
                {
                    runner = new ExpectedResultTestRunner(testStep.ExpectedResult, t => CreateTestRunner(t, t.MethodInfo, null, null),
                                 MessageBus, ConstructorArguments, skip ? skipReason : string.Empty, Aggregator, CancellationTokenSource, _testNotificationType);
                    runSummary.Aggregate(await runner.RunAsync());
                    skip = runSummary.Failed > 0;
                }
            }

            foreach (var cleanup in _testCaseDefinition.Cleanups)
            {
                var runner = CreateTestRunner(cleanup, cleanup.MethodInfo, null, _testNotificationType);
                runSummary.Aggregate(await runner.RunAsync());
            }

            return runSummary;
        }

        private async Task<RunSummary> RunTestCaseSummary(bool hasErrors)
        {
            if (hasErrors)
                return FailBecauseOfException(new TestCaseFailedException("The test case steps were not completed successfully."));

            _test.Instance = _testClassInstance;
            var runner = CreateTestRunner(_test, TestCase.Method, null, _testNotificationType);
            return await runner.RunAsync();
        }

        private TestRunner CreateTestRunner(ITest test, IMethodInfo testMethod, string skipReason, Type testNotificationType)
        {
            var method = testMethod.ToRuntimeMethod();
            return new TestRunner(test, MessageBus, ConstructorArguments, method, skipReason, new ExceptionAggregator(Aggregator), CancellationTokenSource, testNotificationType);
        }

        private RunSummary FailBecauseOfException(Exception exception)
        {
            var test = new XunitTest(new TestCaseWithoutTraits(TestCase, TestCase.Method), DisplayName);
            return FailTestBecauseOfException(test, exception);
        }

        private RunSummary FailTestBecauseOfException(Xunit.Abstractions.ITest test, Exception exception)
        {
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