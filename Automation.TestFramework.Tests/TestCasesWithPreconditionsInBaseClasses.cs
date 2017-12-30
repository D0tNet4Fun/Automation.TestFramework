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

    [TestCase("with preconditions in base class chain")]
    public class TestCaseWithPreconditionsInBaseClass2 : TestCaseBase2
    {
        [Summary("With preconditions in base class chain")]
        public void Summary()
        {

        }

        [Input(1)]
        public void Input1() => Assert.Equal(2, CallCount);
    }

    public class TestCaseBase2 : TestCaseBase
    {
        [Precondition(2)]
        public void Precondition2() => CallCount++;
    }
}