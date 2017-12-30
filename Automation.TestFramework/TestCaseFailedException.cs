using System;

namespace Automation.TestFramework
{
    /// <summary>
    /// The exception thrown when a test case is not successfully completed.
    /// </summary>
    [Serializable]
    public class TestCaseFailedException : Exception
    {
        public TestCaseFailedException(string message)
            : base(message)
        {
        }

        public override string StackTrace => string.Empty;
    }
}