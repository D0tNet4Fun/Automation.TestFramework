using System;
using Xunit;

namespace Automation.TestFramework.Tests
{
    [TestCase("id")]
    public class ExpectedResultTestCaseWithVerifyWhenAllFail
    {
        [Summary("Expected result with verify when all fail")]
        public void Summary() { }

        [Input]
        public void Input() { }

        [ExpectedResult]
        public IExpectedResult ExpectedResult()
        {
            return new ExpectedResult()
                .Verify("This should fail but it should not stop the test", () => Assert.True(1 == 2))
                .Verify("This should fail too", () => Assert.True(2 == 3));
        }
    }
}