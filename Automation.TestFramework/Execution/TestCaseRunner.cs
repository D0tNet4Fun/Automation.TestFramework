using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Automation.TestFramework.Discovery;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Automation.TestFramework.Execution
{
    internal class TestCaseRunner : TestCaseRunner<IXunitTestCase>
    {
        private object _testClassInstance;
        private readonly List<TestRunner> _testRunners;
        private ITest _test; // the test bound to the test case

        public TestCaseRunner(IXunitTestCase testCase, string displayName, string skipReason, object[] constructorArguments, IMessageBus messageBus, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
            : base(testCase, messageBus, aggregator, cancellationTokenSource)
        {
            DisplayName = displayName;
            _testRunners = new List<TestRunner>();
            SkipReason = skipReason;
            ConstructorArguments = constructorArguments;
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
            _test = new Test(TestCase, TestCase.Method, DisplayName);
            var timer = new ExecutionTimer();
            Aggregator.Run(() => _testClassInstance = _test.CreateTestClass(TestClass, ConstructorArguments, MessageBus, timer, CancellationTokenSource));

            // discover the other tests
            Aggregator.Run(() =>
            {
                var testDiscoverer = new TestDiscoverer(TestCase);
                _testRunners.AddRange(testDiscoverer.DiscoverTests().Select(test => CreateTestRunner(test, test.MethodInfo)));
            });
        }

        protected override Task BeforeTestCaseFinishedAsync()
        {
            var test = new XunitTest(TestCase, DisplayName);
            var timer = new ExecutionTimer();
            Aggregator.Run(() => test.DisposeTestClass(_testClassInstance, MessageBus, timer, CancellationTokenSource));

            return base.BeforeTestCaseFinishedAsync();
        }

        protected override async Task<RunSummary> RunTestAsync()
        {
            var runSummary = new RunSummary();

            foreach (var testRunner in _testRunners)
            {
                runSummary.Aggregate(await testRunner.RunAsync());
            }

            var runner = CreateTestRunner(_test, TestCase.Method);
            runSummary.Aggregate(await runner.RunAsync());

            return runSummary;
        }

        private TestRunner CreateTestRunner(ITest test, IMethodInfo testMethod)
        {
            var method = (testMethod as IReflectionMethodInfo).MethodInfo;
            return new TestRunner(_testClassInstance, test, MessageBus, TestClass, method, new ExceptionAggregator(Aggregator), CancellationTokenSource);
        }
    }
}