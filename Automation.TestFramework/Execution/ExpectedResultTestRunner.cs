using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Automation.TestFramework.Entities;
using Xunit.Sdk;

namespace Automation.TestFramework.Execution
{
    internal class ExpectedResultTestRunner : TestRunner, ITestRunner
    {
        private readonly ITest _test;
        private readonly Func<ITest, ITestRunner> _testRunnerFactory;
        private readonly RunSummary _assertionSummary; // used to track execution of assertions, if any

        public ExpectedResult ExpectedResult { get; }

        public ExpectedResultTestRunner(ITest test, Func<ITest, ITestRunner> testRunnerFactory,
            IMessageBus messageBus, object[] constructorArguments, string skipReason, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource, Type testNotificationType)
            : base(test, messageBus, constructorArguments, test.MethodInfo.ToRuntimeMethod(), skipReason, aggregator, cancellationTokenSource, testNotificationType)
        {
            _test = test;
            _testRunnerFactory = testRunnerFactory;
            _assertionSummary = new RunSummary();
            ExpectedResult = new ExpectedResult();
        }

        public new async Task<RunSummary> RunAsync()
        {
            var summary = await base.RunAsync();
            summary.Aggregate(_assertionSummary);
            return summary;
        }

        protected override void InitializeTestStep()
        {
            base.InitializeTestStep();
            TestStep.Current.ExpectedResult = ExpectedResult;
        }

        protected override async Task<decimal> InvokeTestMethodAsync(ExceptionAggregator aggregator)
        {
            // run the test first to collect all the expected result's assertions; if it fails then exit.
            var result = await base.InvokeTestMethodAsync(aggregator);
            if (aggregator.HasExceptions) return result;

            // check if there are assertions defined; if not then return.
            var assertions = ExpectedResult.Assertions;
            if (assertions.Length == 0) return result;

            // run the assertions as individual tests
            var count = await ExecuteAssertionsAsync(assertions);

            // if there are errors then throw a generic exception in the end
            if (_assertionSummary.Failed > 0)
            {
                var errorMessage = "One or more of the assertions failed.";
                if (count < assertions.Length) errorMessage += $" {assertions.Length - count} assertion(s) were skipped.";
                throw new ExpectedResultFailedException(errorMessage);
            }

            return result;
        }

        private async Task<int> ExecuteAssertionsAsync(IEnumerable<ExpectedResultAssertion> assertions)
        {
            var index = 1;
            foreach (var assertion in assertions)
            {
                // testWithExpectedResult.DisplayNamePrefix = prefix + " " + test.DisplayName.Substring(0, test.DisplayName.IndexOf(".") + 1); // i.e. [2/2] [Expected result] 1.

                //var displayName = $"{_test.DisplayNamePrefix}{index++}. {assertion.Description}";
                var displayName = $"{index++}. {assertion.Description}";
                var test = new ExpectedResultTest(_test.TestCase, assertion.Action.Target, new ReflectionMethodInfo(assertion.Action.Method), displayName, assertion.ContinueOnError);
                var runner = _testRunnerFactory(test);
                _assertionSummary.Aggregate(await runner.RunAsync());
                if (_assertionSummary.Failed > 0 && !assertion.ContinueOnError)
                    break;
            }
            return index - 1; // how many actions were executed
        }
    }
}