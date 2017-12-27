using System;

namespace Automation.TestFramework.Tests
{
    [TestCase("WithErrors-1")]
    public class TestCaseWithErrors1
    {
        [Summary("sum")]
        public void Summary()
        {

        }

        [Precondition(1, "prec")]
        public void Precondition1()
        {
        }

        [Input(1, "input")]
        public void Input1()
        {
            throw new Exception("Error thrown");
        }

        [ExpectedResult(1, "expected")]
        public void Expected1()
        {
        }
    }
}