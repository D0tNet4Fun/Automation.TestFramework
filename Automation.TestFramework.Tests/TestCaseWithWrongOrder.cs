namespace Automation.TestFramework.Tests
{
    [TestCase("wrong order")]
    public class TestCaseWithWrongOrder
    {
        [Summary]
        public void WrongOrderBecauseOrderIsExplicitlySpecified()
        {

        }

        [Precondition(1)]
        public void Precondition1() { }

        [Precondition(1)]
        public void Precondition2() { }

        [Input(1)]
        public void Input1() { }

        [Input(1)]
        public void Input2() { }
    }

    [TestCase("wrong order 2")]
    public class TestCaseWithWrongOrder2
    {
        [Summary]
        public void WrongOrderBecauseOrderIsNotSpecified()
        {

        }

        [Precondition]
        public void Precondition1() { }

        [Precondition]
        public void Precondition2() { }

        [Input]
        public void Input1() { }

        [Input]
        public void Input2() { }
    }
}