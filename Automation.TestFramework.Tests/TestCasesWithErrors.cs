using System;
using Xunit;

namespace Automation.TestFramework.Tests
{
    [TestCase("WithErrors-1")]
    public class TestCaseWithErrors1 : IDisposable
    {
        private bool _isExpectedCalled;
        private bool _isSummaryCalled;

        [Summary("Summary - fails")]
        public void Summary()
        {
            _isSummaryCalled = true;
        }

        [Precondition(1, "Precondition")]
        public void Precondition1()
        {
        }

        [Input(1, "Input - fails")]
        public void Input1()
        {
            throw new Exception("Error thrown");
        }

        [ExpectedResult(1, "Expected result - skip")]
        public void Expected1()
        {
            _isExpectedCalled = true;
        }

        public void Dispose()
        {
            Assert.False(_isExpectedCalled);
            Assert.False(_isSummaryCalled);
        }
    }

    [TestCase("WithErrors-2")]
    public class TestCaseWithErrors2 : IDisposable
    {
        private bool _isExpectedCalled;
        private bool _isSummaryCalled;
        private bool _isInputCalled;

        [Summary("Summary - fails")]
        public void Summary()
        {
            _isSummaryCalled = true;
        }

        [Precondition(1, "Precondition - fails")]
        public void Precondition1()
        {
            throw new Exception("Error thrown");
        }

        [Input(1, "Input - skip")]
        public void Input1()
        {
            _isInputCalled = true;
        }

        [ExpectedResult(1, "Expected result - skip")]
        public void Expected1()
        {
            _isExpectedCalled = true;
        }

        public void Dispose()
        {
            Assert.False(_isExpectedCalled);
            Assert.False(_isInputCalled);
            Assert.False(_isSummaryCalled);
        }
    }
}