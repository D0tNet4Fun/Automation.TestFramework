using System;
using Xunit;

namespace Automation.TestFramework.Tests
{
    [TestCase("id")]
    [TestNotification(typeof(Notification))]
    public class ExpectedResultTestCaseWithNotifications : IDisposable
    {
        public bool CalledByNotification { get; private set; }

        [Summary("Expected result test case with notifications")]
        public void Summary()
        {

        }

        [Input]
        private void Input() { }

        [ExpectedResult]
        private void ExpectedResult()
        {
            TestStep.Current.ExpectedResult
                .Assert("This should fail", () => Assert.True(1 == 2));
        }

        public void Dispose()
        {
            Assert.False(CalledByNotification);
        }

        public class Notification : ITestNotification
        {
            private readonly ExpectedResultTestCaseWithNotifications _testClassInstance;

            public Notification(object testClassInstance)
            {
                _testClassInstance = (ExpectedResultTestCaseWithNotifications)testClassInstance;
            }

            public void OnError(Exception error)
            {
                _testClassInstance.CalledByNotification = true;
            }
        }
    }
}