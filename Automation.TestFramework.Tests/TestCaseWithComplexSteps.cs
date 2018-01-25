using Xunit;

namespace Automation.TestFramework.Tests
{
    [TestCase("id")]
    public class TestCaseWithComplexSteps
    {
        [Summary]
        public void Summary()
        {

        }

        [Input(1, "Given this")]
        public void Input1()
        {

        }

        [Input(2, "And given that")]
        public void Input2()
        {

        }

        [ExpectedResult(2)]
        public ITestStep ExpectedResult2()
        {
            object context = null;
            return new TestStep()
                .Initialize(() => context = new object())
                .Do("I expect this to work", () => Assert.NotNull(context))
                .Do("And I expect that to work too", () => Assert.True(1 != 0));
        }
    }
}