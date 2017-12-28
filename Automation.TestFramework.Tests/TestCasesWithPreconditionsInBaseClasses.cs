using Xunit;

namespace Automation.TestFramework.Tests
{
    public class TestCaseBase
    {
        public int CallCount { get; set; }

        [Precondition(1)]
        public void Precondition1()
        {
            CallCount++;
        }
    }

    [TestCase("with preconditions in base class")]
    public class TestCaseWithPreconditionsInBaseClass1 : TestCaseBase
    {
        [Summary("With preconditions in base class")]
        public void Summary()
        {

        }

        [Input(1)]
        public void Input1() => Assert.Equal(1, CallCount);
    }
}