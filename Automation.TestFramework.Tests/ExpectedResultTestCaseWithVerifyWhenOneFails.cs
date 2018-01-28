using System;
using Xunit;

namespace Automation.TestFramework.Tests
{
    [TestCase("id")]
    public class ExpectedResultTestCaseWithVerifyWhenOneFails : IDisposable
    {
        private bool _called;

        [Summary("Expected result with verify when one fails")]
        public void Summary() { }

        [Input]
        public void Input() { }

        [ExpectedResult]
        public void ExpectedResult()
        {
            TestStep.Current.ExpectedResult
                .Verify("This should fail but it should not stop the test", () => Assert.True(1 == 2))
                .Verify("This should work", () =>
                {
                    _called = true;
                    Assert.True(1 != 2);
                });
        }

        public void Dispose()
        {
            Assert.True(_called);
        }
    }
}