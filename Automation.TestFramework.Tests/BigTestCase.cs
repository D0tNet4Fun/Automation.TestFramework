namespace Automation.TestFramework.Tests
{
    [TestCase("Big")]
    public class BigTestCase
    {
        [Summary]
        public void Summary()
        {
        }

        [Input(1)] private void Input1() { }
        [Input(2)] private void Input2() { }
        [Input(3)] private void Input3() { }
        [Input(4)] private void Input4() { }
        [Input(5)] private void Input5() { }
        [Input(6)] private void Input6() { }
        [Input(7)] private void Input7() { }
        [Input(8)] private void Input8() { }
        [Input(9)] private void Input9() { }
        [Input(10)] private void Input10() { }
        [Input(11)] private void Input11() { }
        [Input(12)] private void Input12() { }
        [ExpectedResult(12)] private void ExpectedResult() { }

    }
}