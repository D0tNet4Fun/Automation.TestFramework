using System;
using Xunit;

namespace Automation.TestFramework.Tests
{
    [TestCase("id")]
    public class ExpectedResultTestCaseWithVerifyWhenTestMethodFails : IDisposable
    {
        private bool _called;

        [Summary("Expected result with verify when test method fails")]
        public void Summary() { }

        [Input]
        public void Input() { }

        [ExpectedResult]
        public ExpectedResult ExpectedResult()
        {
            Throw();
            return new ExpectedResult()
                .Verify("This will never be called", () => _called = true);
        }

        private void Throw()
        {
            throw new Exception("error");
        }

        public void Dispose()
        {
            Assert.False(_called);
        }
    }
}