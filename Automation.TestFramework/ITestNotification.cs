using System;

namespace Automation.TestFramework
{
    /// <summary>
    /// Defines methods for getting notifications while a test is executed.
    /// </summary>
    public interface ITestNotification
    {
        /// <summary>
        /// Called when an error occurs.
        /// </summary>
        /// <param name="error">The error.</param>
        void OnError(Exception error);
    }
}