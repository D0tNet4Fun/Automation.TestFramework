using System;
using Xunit;

namespace Automation.TestFramework.Tests.Notifications
{
    [TestCase("ID")]
    public class TestCaseWithErrorsAndNotification : IDisposable
    {
        public static Exception Exception { get; } = new Exception("error");

        [Summary("Test case with errors and notification")]
        public void Summary()
        {

        }

        [Input(1)]
        public void InputThrowsException() => throw Exception; // this should be rethrown in GlobalTestNotification

        public void Dispose()
        {
            // the instance passed to GlobalTestNotification should be this
            Assert.Same(this, GlobalTestNotification.Instance.TestClassInstance);
            // the exception passed to GlobalTestNotification should be this one
            Assert.Same(Exception, GlobalTestNotification.Instance.Error);
        }
    }
}