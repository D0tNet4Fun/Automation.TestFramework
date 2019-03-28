using System;
using System.Collections.Generic;

namespace Automation.TestFramework
{
    /// <summary>
    /// The exception thrown when an expected result is not defined correctly.
    /// </summary>
    [Serializable]
    public class ExpectedResultFailedException : AggregateException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpectedResultFailedException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerExceptions">The exceptions that caused the error.</param>
        public ExpectedResultFailedException(string message, IEnumerable<Exception> innerExceptions)
            : base(string.Empty, innerExceptions)
        {
            Message = message;
            StackTrace = string.Empty;
        }

        /// <inheritdoc/>
        public override string Message { get; }

        /// <inheritdoc />
        public override string StackTrace { get; }
    }
}