using System;
using Xunit;

namespace Automation.TestFramework.Tests
{
    public class SetupCleanupFixture
    {
        [Setup(1)]
        public void Setup() => IsSetupCalled = true;

        [Cleanup(1)]
        public void Cleanup() => IsCleanupCalled = true;

        public bool IsSetupCalled { get; set; }
        public bool IsCleanupCalled { get; set; }
    }

    [TestCase("ID")]
    public class TestCaseWithSetupAndCleanupInFixture : IClassFixture<SetupCleanupFixture>, IDisposable
    {
        private readonly SetupCleanupFixture _fixture;

        public TestCaseWithSetupAndCleanupInFixture(SetupCleanupFixture fixture)
        {
            _fixture = fixture;
        }

        [Summary("test case with setup and cleanup in fixture")]
        public void Summary()
        {
        }

        [Input(1)]
        public void Input()
        {

        }

        public void Dispose()
        {
            Assert.True(_fixture.IsSetupCalled);
            Assert.True(_fixture.IsCleanupCalled);
        }
    }

    [TestCase("ID")]
    public class TestCaseWithSetupAndCleanupInFixtureWhenError : IClassFixture<SetupCleanupFixture>, IDisposable
    {
        private readonly SetupCleanupFixture _fixture;

        public TestCaseWithSetupAndCleanupInFixtureWhenError(SetupCleanupFixture fixture)
        {
            _fixture = fixture;
        }

        [Summary("test case with setup and cleanup in fixture when error")]
        public void Summary()
        {
        }

        [Input(1)]
        public void Input() => throw new Exception("error");

        public void Dispose()
        {
            Assert.True(_fixture.IsSetupCalled);
            Assert.True(_fixture.IsCleanupCalled);
        }
    }
}