using System;
using System.Collections.Generic;
using Automation.TestFramework.Entities;

namespace Automation.TestFramework
{
    public interface IExpectedResult
    {
        IExpectedResult Assert(string description, Action action);

        IExpectedResult Verify(string description, Action action);
    }

    public class ExpectedResult : IExpectedResult
    {
        private readonly List<ExpectedResultAction> _actions = new List<ExpectedResultAction>();

        public ExpectedResult()
        {
            Current = this;
        }

        [ThreadStatic] internal static ExpectedResult Current;

        /// <summary>
        /// Make an assertion. If it proves to be false then the next assertions or verifications will not be executed and the test will fail.
        /// </summary>
        /// <param name="description">The description of the assertion.</param>
        /// <param name="action">The action which defines the assertion.</param>
        /// <returns>This instance.</returns>
        public IExpectedResult Assert(string description, Action action)
        {
            RequireDescription(description);
            _actions.Add(new ExpectedResultAction(description, action, continueOnError: false));
            return this;
        }

        /// <summary>
        /// Make a verification. If it proves to be false then the next assertions or verifications will continue to be executed. The test will fail in the end.
        /// </summary>
        /// <param name="description">The description of the verification.</param>
        /// <param name="action">The action which defines the verification.</param>
        /// <returns>This instance.</returns>
        public IExpectedResult Verify(string description, Action action)
        {
            RequireDescription(description);
            _actions.Add(new ExpectedResultAction(description, action, continueOnError: true));
            return this;
        }

        private void RequireDescription(string description)
        {
            if (string.IsNullOrEmpty(description))
                throw new ArgumentException("Description must not be empty", nameof(description));
        }

        internal ExpectedResultAction[] Actions => _actions.ToArray();

        internal void AssignDisplayNameComponents(string prefix, int order)
        {

        }

        internal void UpdateActionsDisplayName()
        {
            foreach (var action in _actions)
            {

            }
        }
    }
}