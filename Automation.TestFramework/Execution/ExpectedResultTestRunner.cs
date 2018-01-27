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
        private readonly ITestWithExpectedResult _test;
        private readonly Func<ITest, ITestRunner> _testRunnerFactory;
        private readonly Func<ITest, Exception, RunSummary> _failTestOnExceptionCallback;
        private readonly RunSummary _assertionSummary; // used to track execution of assertions

        public ExpectedResultTestRunner(ITestWithExpectedResult test, Func<ITest, ITestRunner> testRunnerFactory, Func<ITest, Exception, RunSummary> failTestOnExceptionCallback,
            IMessageBus messageBus, object[] constructorArguments, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource, Type testNotificationType)
            : base(test, messageBus, constructorArguments, test.MethodInfo.ToRuntimeMethod(), string.Empty, aggregator, cancellationTokenSource, testNotificationType)
        {
            _test = test;
            _testRunnerFactory = testRunnerFactory;
            _failTestOnExceptionCallback = failTestOnExceptionCallback;
            _assertionSummary = new RunSummary();
        }

        public new async Task<RunSummary> RunAsync()
        {
            var summary = await base.RunAsync();
            summary.Aggregate(_assertionSummary);
            return summary;
        }

        protected override async Task<decimal> InvokeTestMethodAsync(ExceptionAggregator aggregator)
        {
            // run the test first to collect all the expected result's assertions; if it fails then exit.
            var result = await base.InvokeTestMethodAsync(aggregator);
            if (aggregator.HasExceptions) return result;

            var assertions = ExpectedResult.Current.Assertions;
            if (assertions.Length == 0)
            {
                var exception = new ExpectedResultFailedException($"Expected result should have at least one assertion ({nameof(IExpectedResult.Assert)} or {nameof(IExpectedResult.Verify)})");
                _assertionSummary.Aggregate(_failTestOnExceptionCallback(_test, exception));
                return result;
            }

            var count = await ExecuteAssertionsAsync(assertions);

            // if there are errors then throw a generic exception in the end
            if (_assertionSummary.Failed > 0)
            {
                var errorMessage = "One or more of the expected results did not match.";
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
                var displayName = $"{_test.DisplayNamePrefix}{index++}. {assertion.Description}";
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