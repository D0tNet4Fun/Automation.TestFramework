using System;
using System.Threading.Tasks;
using Xunit;

namespace Automation.TestFramework.Tests.Notifications
{
    [TestCase("ID")]
    public class TestCaseWithErrorsInAsyncMethodsAndNotification: IDisposable
    {
        public static Exception Exception { get; } = new Exception("error");

        [Summary("Test case with errors in async methods and notification")]
        public void Summary()
        {

        }

        [Input(1)]
        public async Task InputThrowsException()
        {
            await Task.Delay(1);
            throw Exception;
            // this should be rethrown in GlobalTestNotification
        }

        public void Dispose()
        {
            // the instance passed to GlobalTestNotification should be this
            Assert.Same(this, GlobalTestNotification.Instance.TestClassInstance);
            // the exception passed to GlobalTestNotification should be this one
            Assert.Same(Exception, GlobalTestNotification.Instance.Error);
        }
    }
}