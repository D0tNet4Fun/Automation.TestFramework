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

    [TestCase("ID")]
    [TestNotification(typeof(ClassTestNotification))]
    public class TestCaseWithErrorsInsideExpectedResultsAndClassNotification : IDisposable
    {
        public static Exception Exception1 { get; } = new Exception("error 1");
        public static Exception Exception2 { get; } = new Exception("error 2");

        [Summary("Test case with errors in assertions and class notification")]
        public void Summary()
        {

        }

        [Input(1)]
        public void Input()
        {
            
        }

        [ExpectedResult(1)]
        public void ExpectedResult()
        {
            TestStep.Current.ExpectedResult
                .Verify("fail 1", () => throw Exception1)
                .Verify("fail 2", () => throw Exception2)
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
            Assert.Null(GlobalTestNotification.Instance);
        }
    }
}