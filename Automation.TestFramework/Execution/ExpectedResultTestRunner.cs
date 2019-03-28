using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Automation.TestFramework.Entities;
using Xunit.Sdk;

namespace Automation.TestFramework.Execution
{
    internal class ExpectedResultTestRunner : TestRunner
    {
        private readonly Type _testNotificationType;
        private readonly object _testClassInstance;
        public ExpectedResult ExpectedResult { get; }

        public ExpectedResultTestRunner(ITest test, Func<ITest, TestRunner> testRunnerFactory,
            IMessageBus messageBus, object[] constructorArguments, string skipReason, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource, Type testNotificationType, object testClassInstance)
            : base(test, messageBus, constructorArguments, test.MethodInfo.ToRuntimeMethod(), skipReason, aggregator, cancellationTokenSource, testNotificationType)
        {
            _testNotificationType = testNotificationType;
            _testClassInstance = testClassInstance;
            ExpectedResult = new ExpectedResult(test, testRunnerFactory);
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

            // check if the expected result has errors; if so then throw a generic exception in the end
            if (ExpectedResult.RunSummary.Failed > 0)
            {
                var exception = CreateExpectedResultFailedException();
                if (_testNotificationType != null) TryNotify(exception);
                throw exception;
            }

            return result;
        }

        private ExpectedResultFailedException CreateExpectedResultFailedException()
        {
            var errorMessage = "One or more of the assertions failed.";
            if (ExpectedResult.RunSummary.Skipped > 0) errorMessage += $" {ExpectedResult.RunSummary.Skipped} assertion(s) were skipped.";

            IReadOnlyCollection<Exception> exceptions;
            var exception = ExpectedResult.ExceptionAggregator.ToException();
            if (exception is AggregateException aggregateException)
            {
                exceptions = aggregateException.InnerExceptions;
            }
            else
            {
                exceptions = new[] { exception };
            }

            return new ExpectedResultFailedException(errorMessage, exceptions);
        }

        private void TryNotify(Exception exception)
        {
            try
            {
                var notification = (ITestNotification)Activator.CreateInstance(_testNotificationType, _testClassInstance);
                notification.OnError(exception);
            }
            catch (Exception inner)
            {
                var error = $"Error in test notification of type {_testNotificationType.Name}: {inner}";
                // todo where to output this?
            }
        }
    }
}