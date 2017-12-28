using System;
using Xunit;

namespace Automation.TestFramework.Tests
{
    [TestCase("ID")]
    public class TestCaseWithSetupAndCleanup : IDisposable
    {
        private bool _isSetupCalled;
        private bool _isCleanupCalled;

        [Summary("test case with setup and cleanup")]
        public void Summary()
        {
        }

        [Setup(1)]
        public void Setup() => _isSetupCalled = true;

        [Input(1)]
        public void Input()
        {

        }

        [Cleanup(1)]
        public void Cleanup() => _isCleanupCalled = true;

        public void Dispose()
        {
            Assert.True(_isSetupCalled);
            Assert.True(_isCleanupCalled);
        }
    }

    [TestCase("ID")]
    public class TestCaseWithSetupAndCleanupWhenError : IDisposable
    {
        private bool _isSetupCalled;
        private bool _isCleanupCalled;

        [Summary("test case with setup and cleanup when error")]
        public void Summary()
        {
        }

        [Setup(1)]
        public void Setup() => _isSetupCalled = true;

        [Input(1)]
        public void Input() => throw new Exception("error");

        [Cleanup(1)]
        public void Cleanup() => _isCleanupCalled = true;

        public void Dispose()
        {
            Assert.True(_isSetupCalled);
            Assert.True(_isCleanupCalled);
        }
    }
}