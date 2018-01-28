namespace Automation.TestFramework.Tests
{
    [TestCase("id")]
    public class ExpectedResultTestCaseWhenExpectedResultDoesNotHaveAssertions
    {
        [Summary("Expected result without assertions")]
        public void Summary() { }

        [Input]
        private void Input() { }

        [ExpectedResult]
        private ExpectedResult ExpectedResult()
        {
            return new ExpectedResult();
        }
    }

    [TestCase("id")]
    public class ExpectedResultTestCaseWhenAnotherTestStepReturnsAnExpectedResult
    {
        [Summary("Expected result with input returning " + nameof(TestFramework.ExpectedResult))]
        public void Summary() { }

        [Input]
        private ExpectedResult Input() => new ExpectedResult();

        [ExpectedResult]
        private void ExpectedResult() { }
    }
}