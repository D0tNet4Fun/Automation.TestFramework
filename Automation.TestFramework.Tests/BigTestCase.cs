namespace Automation.TestFramework.Tests
{
    [TestCase("Big")]
    public class BigTestCase
    {
        [Summary]
        public void Summary()
        {
        }

        [Input] private void Input1() { }
        [Input] private void Input2() { }
        [Input] private void Input3() { }
        [Input] private void Input4() { }
        [Input] private void Input5() { }
        [Input] private void Input6() { }
        [Input] private void Input7() { }
        [Input] private void Input8() { }
        [Input] private void Input9() { }
        [Input] private void Input10() { }
        [Input] private void Input11() { }
        [Input] private void Input12() { }
        [ExpectedResult] private void ExpectedResult() { }

    }
}