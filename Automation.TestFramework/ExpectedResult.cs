using System;
using System.Collections.Generic;
using Automation.TestFramework.Entities;

namespace Automation.TestFramework
{
    /// <summary>
    /// Represents an expected result that has multiple assertions.
    /// </summary>
    public class ExpectedResult
    {
        private readonly List<ExpectedResultAssertion> _assertions = new List<ExpectedResultAssertion>();

        /// <summary>
        /// Make an assertion whose failure stops the execution of the test case step.
        /// </summary>
        /// <param name="description">The description of the assertion.</param>
        /// <param name="action">The action which defines the assertion.</param>
        /// <returns>This instance.</returns>
        public ExpectedResult Assert(string description, Action action)
        {
            RequireDescription(description);
            _assertions.Add(new ExpectedResultAssertion(description, action, continueOnError: false));
            return this;
        }

        /// <summary> 
        /// Make an assertion whose failure does not stop the execution of the test case step, allowing the next assertions to execute. 
        /// The test step will fail in the end.
        /// </summary>
        /// <param name="description">The description of the assertion.</param>
        /// <param name="action">The action which defines the assertion.</param>
        /// <returns>This instance.</returns>
        public ExpectedResult Verify(string description, Action action)
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