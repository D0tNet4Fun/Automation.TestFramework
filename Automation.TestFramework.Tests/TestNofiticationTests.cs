using System;
using Automation.TestFramework;
using Automation.TestFramework.Tests;
using Xunit;

[assembly: TestNotification(typeof(GlobalTestNotification))]


namespace Automation.TestFramework.Tests
{
    public class GlobalTestNotification : ITestNotification
    {
        public GlobalTestNotification(object testClassInstance)
        {
            Instance = this;
            TestClassInstance = testClassInstance;
        }

        public static GlobalTestNotification Instance { get; set; }
        public object TestClassInstance { get; set; }
        public Exception Error { get; set; }

        public void OnError(Exception error) => Error = error;
    }

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




    public class ClassTestNotification : ITestNotification
    {
        public ClassTestNotification(object testClassInstance)
        {
            Instance = this;
            TestClassInstance = testClassInstance;
        }

        public static ClassTestNotification Instance { get; set; }
        public object TestClassInstance { get; set; }
        public Exception Error { get; set; }

        public void OnError(Exception error) => Error = error;
    }

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
        public void InputThrowsException() => throw Exception; // this should be rethrown in GlobalTestNotification

        public void Dispose()
        {
            // the instance passed to ClassTestNotification should be this
            Assert.Same(this, ClassTestNotification.Instance.TestClassInstance);
            // the exception passed to ClassTestNotification should be this one
            Assert.Same(Exception, ClassTestNotification.Instance.Error);

            // nothing should have been passed to GlobalTestNotification
            Assert.Null(GlobalTestNotification.Instance);
        }
    }
}