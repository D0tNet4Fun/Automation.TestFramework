using System;
using System.Threading.Tasks;
using Xunit;

namespace Automation.TestFramework.Tests.Notifications
{
    [TestCase("ID")]
    [TestNotification(typeof(ClassTestNotification))]
    public class TestCaseWithErrorsInsideExpectedResultsAsyncAndClassNotification : IDisposable
    {
        public static Exception Exception1 { get; } = new Exception("error 1");
        public static Exception Exception2 { get; } = new Exception("error 2");

        [Summary("Test case with errors in assertions async and class notification")]
        public void Summary()
        {

        }

        [Input(1)]
        public void Input()
        {
        }

        [ExpectedResult(1)]
        public async Task ExpectedResult()
        {
            await Task.Delay(1);
            TestStep.Current.ExpectedResult
                .Verify("fail 1", async () =>
                {
                    await Task.Delay(1);
                    throw Exception1;
                })
                .Verify("fail 2", async () =>
                {
                    await Task.Delay(1);
                    throw Exception2;
                })
                ;
        }

        public void Dispose()
        {
            // the instance passed to ClassTestNotification should be this
            Assert.Same(this, ClassTestNotification.Instance.TestClassInstance);

            // ExpectedResultFailedException should be passed to ClassTestNotification and contain the 2 errors thrown in assertions
            var error = ClassTestNotification.Instance.Error as ExpectedResultFailedException;
            Assert.NotNull(error);
            Assert.Same(Exception1, error.InnerExceptions[0]);
            Assert.Same(Exception2, error.InnerExceptions[1]);

            // nothing should have been passed to GlobalTestNotification
            Assert.Null(GlobalTestNotification.Instance?.Error);
        }
    }
}