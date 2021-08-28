using System;
using System.Threading.Tasks;
using Xunit;

namespace Automation.TestFramework.Tests.Notifications
{
    [TestCase("ID")]
    [TestNotification(typeof(ClassTestNotification))]
    public class TestCaseWithErrorsInAsyncMethodsAndClassNotification : IDisposable
    {
        public static Exception Exception { get; } = new Exception("error");

        [Summary("Test case with errors in async methods and class notification")]
        public void Summary()
        {

        }

        [Input(1)]
        public async Task InputThrowsException()
        {
            await Task.Delay(1);
            throw Exception;
            // this should be rethrown in ClassTestNotification
        }

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