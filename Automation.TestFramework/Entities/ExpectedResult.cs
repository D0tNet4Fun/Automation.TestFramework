using System;
using Automation.TestFramework.Execution;
using Xunit.Sdk;

namespace Automation.TestFramework.Entities
{
    /// <summary>
    /// Defines methods to specify multiple assertions for an expected result.
    /// </summary>
    public interface IExpectedResult
    {
        /// <summary>
        /// Make an assertion whose failure stops the execution of the test case step.
        /// </summary>
        /// <param name="description">The description of the assertion.</param>
        /// <param name="action">The action which defines the assertion.</param>
        /// <returns>This instance.</returns>
        IExpectedResult Assert(string description, Action action);

        /// <summary> 
        /// Make an assertion whose failure does not stop the execution of the test case step, allowing the next assertions to execute. 
        /// The test step will fail in the end.
        /// </summary>
        /// <param name="description">The description of the assertion.</param>
        /// <param name="action">The action which defines the assertion.</param>
        /// <returns>This instance.</returns>
        IExpectedResult Verify(string description, Action action);
    }

    /// <summary>
    /// Represents an expected result that has multiple assertions.
    /// </summary>
    internal class ExpectedResult : IExpectedResult
    {
        private readonly ITest _test;
        private readonly Func<ITest, TestRunner> _testRunnerFactory;

        public ExceptionAggregator ExceptionAggregator { get; }

        public ExpectedResult(ITest test, Func<ITest, TestRunner> testRunnerFactory)
        {
            _test = test;
            _testRunnerFactory = testRunnerFactory;
            RunSummary = new RunSummary();
            ExceptionAggregator = new ExceptionAggregator();
        }

        public RunSummary RunSummary { get; }

        public bool HasCriticalError { get; private set; }

        /// <inheritdoc />
        public IExpectedResult Assert(string description, Action action)
        {
            ExecuteOrSkipAssertion(description, action, continueOnError: false);
            return this;
        }

        /// <inheritdoc />
        public IExpectedResult Verify(string description, Action action)
        {
            ExecuteOrSkipAssertion(description, action, continueOnError: true);
            return this;
        }

        private void RequireDescription(string description)
        {
            if (string.IsNullOrEmpty(description))
                throw new ArgumentException("Description must not be empty", nameof(description));
        }

        private void ExecuteOrSkipAssertion(string description, Action action, bool continueOnError)
        {
            // if there is an error then do not execute this assertion, just track it as skipped
            if (HasCriticalError)
            {
                RunSummary.Total++;
                RunSummary.Skipped++;
                return;
            }

            RequireDescription(description);
            var displayNamePrefix = _test.DisplayName.Substring(0, _test.DisplayName.IndexOf(".") + 1);// i.e. [2/2] [Expected result] 1.
            var displayName = $"{displayNamePrefix}{RunSummary.Total + 1}. {description}";// i.e. [2/2] [Expected result] 1.1. Expect that...

            var test = new ExpectedResultTest(_test.TestCase, action.Target, new ReflectionMethodInfo(action.Method), displayName, continueOnError);
            var testRunner = _testRunnerFactory(test);
            testRunner.ExceptionAggregator = ExceptionAggregator; // to persist the exceptions
            RunSummary.Aggregate(testRunner.RunAsync().GetAwaiter().GetResult());

            // if the assertion failed and we are not allowed to continue on error, then ensure all future assertions will be skipped
            if (RunSummary.Failed > 0 && !continueOnError)
            {
                HasCriticalError = true;
            }
        }
    }
}