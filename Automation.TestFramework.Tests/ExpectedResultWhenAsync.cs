using System;
using System.Threading.Tasks;
using Xunit;

namespace Automation.TestFramework.Tests
{
    [TestCase("ExpectedResultWhenAsync")]
    public class ExpectedResultWhenAsync
    {
        [Summary("Expected result when async")]
        public void Summary() { }

        [Input]
        public void Input()
        {
            Assert.Same(TestStep.GetCurrent(), TestStep.Current);
        }

        [ExpectedResult]
        public async Task ExpectedResult()
        {
            await Task.Delay(1000);
            TestStep.Current.ExpectedResult
                .Assert("This works", () => Assert.True(true));
            await Task.Delay(1000);
            TestStep.Current.ExpectedResult
                .Assert("This works 2", () => Assert.True(true));
            await Task.Delay(1000);
            TestStep.Current.ExpectedResult
                .Assert("This works 3", () => Assert.True(true));
        }
    }
}