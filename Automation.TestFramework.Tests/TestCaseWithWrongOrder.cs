namespace Automation.TestFramework.Tests
{
    [TestCase("wrong order")]
    public class TestCaseWithWrongOrder
    {
        [Summary]
        public void WrongOrder1()
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
}