using System;
using System.Collections.Generic;
using Automation.TestFramework.Entities;

namespace Automation.TestFramework
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
    public class ExpectedResult : IExpectedResult
    {
        private readonly List<ExpectedResultAssertion> _assertions = new List<ExpectedResultAssertion>();

        /// <inheritdoc />
        public IExpectedResult Assert(string description, Action action)
        {
            RequireDescription(description);
            _assertions.Add(new ExpectedResultAssertion(description, action, continueOnError: false));
            return this;
        }

        /// <inheritdoc />
        public IExpectedResult Verify(string description, Action action)
        {
            RequireDescription(description);
            _assertions.Add(new ExpectedResultAssertion(description, action, continueOnError: true));
            return this;
        }

        private void RequireDescription(string description)
        {
            if (string.IsNullOrEmpty(description))
                throw new ArgumentException("Description must not be empty", nameof(description));
        }

        internal ExpectedResultAssertion[] Assertions => _assertions.ToArray();
    }
}