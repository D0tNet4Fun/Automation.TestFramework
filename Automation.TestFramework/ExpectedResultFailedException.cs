using System;

namespace Automation.TestFramework
{
    [Serializable]
    public class ExpectedResultFailedException : Exception
    {
        public ExpectedResultFailedException(string message)
            : base(message)
        {
        }

        public override string StackTrace => string.Empty;
    }
}