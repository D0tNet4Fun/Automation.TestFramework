using System;

namespace Automation.TestFramework
{
    /// <summary>
    /// The exception thrown when a test case is not successfully completed.
    /// </summary>
    [Serializable]
    public class TestCaseFailedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseFailedException" /> class with a specified error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public TestCaseFailedException(string message)
            : base(message)
        {
        }

        /// <inheritdoc />
        public override string StackTrace => string.Empty;
    }
}