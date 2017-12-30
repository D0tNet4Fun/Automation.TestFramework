namespace Automation.TestFramework.Tests
{
    [TestCase("with default order")]
    public class TestCaseWithDefaultOrder
    {
        [Summary] public void Summary() { }

        [Setup] public void Setup() { }

        [Precondition] public void Precondition() { }

        [Input] public void Input() { }

        [ExpectedResult] public void ExpectedResult() { }

        [Cleanup] public void Cleanup() { }
    }
}