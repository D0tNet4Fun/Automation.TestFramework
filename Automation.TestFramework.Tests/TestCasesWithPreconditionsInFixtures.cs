using Xunit;

namespace Automation.TestFramework.Tests
{
    public class TestCaseFixture
    {
        public int CallCount { get; set; }

        [Precondition(1)]
        public void UpdateCallCount1() => CallCount++;

        [Precondition(2)]
        public void UpdateCallCount2() => CallCount++;
    }

    [TestCase("with-fixture-1")]
    public class TestCaseWithFixture1 : IClassFixture<TestCaseFixture>
    {
        private readonly TestCaseFixture _fixture;

        public TestCaseWithFixture1(TestCaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Summary]
        public void WithFixture1()
        {
            Assert.Equal(2, _fixture.CallCount);
        }

        [Input(1)]
        public void Input1()
        {
        }
    }

    [TestCase("with-fixture-2")]
    public class TestCaseWithFixture2 : IClassFixture<TestCaseFixture>
    {
        private readonly TestCaseFixture _fixture;

        public TestCaseWithFixture2(TestCaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Summary]
        public void WithFixture2()
        {
            Assert.Equal(2, _fixture.CallCount);
        }

        [Input(1)]
        public void Input1()
        {
        }
    }
}