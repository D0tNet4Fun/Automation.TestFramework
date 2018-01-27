using System;

namespace Automation.TestFramework.Entities
{
    internal class ExpectedResultAssertion
    {
        public string Description { get; }
        public Action Action { get; }
        public bool ContinueOnError { get; }

        public ExpectedResultAssertion(string description, Action action, bool continueOnError)
        {
            Description = description;
            Action = action;
            ContinueOnError = continueOnError;
        }
    }
}