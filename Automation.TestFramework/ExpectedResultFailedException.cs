using System;

namespace Automation.TestFramework
{
    /// <summary>
    /// The exception thrown when an expected result is not defined correctly.
    /// </summary>
    [Serializable]
    public class ExpectedResultFailedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpectedResultFailedException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ExpectedResultFailedException(string message)
            : base(message)
        {
        }

        /// <inheritdoc />
        public override string StackTrace => string.Empty;
    }
}