namespace Automation.TestFramework.Tests
{
    [TestCase("no descriptions")]
    public class TestCaseWithoutDescriptions
    {
        [Summary]
        public void ThisIsTheTestCaseSummary()
        {
        }

        [Precondition(1)]
        public void ThisIsPrecondition1()
        {

        }

        [Input(1)]
        public void ThisIsTheInput()
        {

        }

        [ExpectedResult(1)]
        public void ThisIsTheExpectedResultForInput1()
        {

        }
    }
}