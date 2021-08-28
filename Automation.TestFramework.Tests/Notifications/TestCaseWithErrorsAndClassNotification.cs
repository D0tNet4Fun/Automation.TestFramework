using System;
using Xunit;

namespace Automation.TestFramework.Tests.Notifications
{
    [TestCase("ID")]
    [TestNotification(typeof(ClassTestNotification))]
    public class TestCaseWithErrorsAndClassNotification : IDisposable
    {
        public static Exception Exception { get; } = new Exception("error");

        [Summary("Test case with errors and class notification")]
        public void Summary()
        {

        }

        [Input(1)]
        public void InputThrowsException() => throw Exception; // this should be rethrown in ClassTestNotification

        public void Dispose()
        {
            // the instance passed to ClassTestNotification should be this
            Assert.Same(this, ClassTestNotification.Instance.TestClassInstance);
            // the exception passed to ClassTestNotification should be this one
            Assert.Same(Exception, ClassTestNotification.Instance.Error);

            // nothing should have been passed to GlobalTestNotification
            Assert.Null(GlobalTestNotification.Instance?.Error);
        }
    }
}