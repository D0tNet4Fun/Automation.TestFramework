using System;
using Xunit;

namespace Automation.TestFramework.Tests
{
    [TestCase("order")]
    public class TestCaseOrder : IDisposable
    {
        private int _count;

        [Summary("Test case components run in order")]
        public void Summary() => Assert.Equal(7, _count++); // this one runs last

        [Precondition(1, "precondition 1")]
        public void Precondition1() => Assert.Equal(0, _count++);

        [Precondition(2, "precondition 2")]
        public void Precondition2() => Assert.Equal(1, _count++);

        [Input(1, "input 1")]
        public void Input1() => Assert.Equal(2, _count++);

        [Input(2, "input 2")]
        public void Input2() => Assert.Equal(3, _count++);

        [ExpectedResult(2, "expected result 2")]
        public void ExpectedResult2() => Assert.Equal(4, _count++);

        [Input(3, "input 3")]
        public void Input3() => Assert.Equal(5, _count++);

        [ExpectedResult(3, "expected result 3")]
        public void ExpectedResult3() => Assert.Equal(6, _count++);

        public void Dispose() => Assert.Equal(8, _count);
    }
}