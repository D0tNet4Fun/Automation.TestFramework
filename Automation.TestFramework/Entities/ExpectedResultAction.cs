using System;

namespace Automation.TestFramework.Entities
{
    internal class ExpectedResultAction
    {
        public string Description { get; }
        public Action Action { get; }
        public bool ContinueOnError { get; }

        public ExpectedResultAction(string description, Action action, bool continueOnError)
        {
            Description = description;
            Action = action;
            ContinueOnError = continueOnError;
        }
    }
}