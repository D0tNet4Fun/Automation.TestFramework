using System;
using Xunit;

namespace Automation.TestFramework.Tests
{
    public class SetupCleanupBase
    {
        [Setup(1)]
        public void Setup() => IsSetupCalled = true;

        [Cleanup(1)]
        public void Cleanup() => IsCleanupCalled = true;

        public bool IsSetupCalled { get; set; }
        public bool IsCleanupCalled { get; set; }
    }

    [TestCase("ID")]
    public class TestCaseWithSetupAndCleanupInBaseClass : SetupCleanupBase, IDisposable
    {

        [Summary("test case with setup and cleanup in base class")]
        public void Summary()
        {
        }

        [Input(1)]
        public void Input()
        {

        }

        public void Dispose()
        {
            Assert.True(IsSetupCalled);
            Assert.True(IsCleanupCalled);
        }
    }

    [TestCase("ID")]
    public class TestCaseWithSetupAndCleanupInBaseClassWhenError : SetupCleanupBase, IDisposable
    {
        [Summary("test case with setup and cleanup in base class when error")]
        public void Summary()
        {
        }

        [Input(1)]
        public void Input() => throw new Exception("error");

        public void Dispose()
        {
            Assert.True(IsSetupCalled);
            Assert.True(IsCleanupCalled);
        }
    }
}