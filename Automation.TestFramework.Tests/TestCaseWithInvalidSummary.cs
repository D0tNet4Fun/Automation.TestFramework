namespace Automation.TestFramework.Tests
{
    [TestCase("no summary")]
    public class TestCaseWithNoSummary
    {

    }

    [TestCase("too many summaries")]
    public class TestCaseWithMoreSummaries
    {
        [Summary]
        public void X()
        {

        }

        [Summary]
        public void Y()
        {

        }
    }
}