using System;
using System.Threading.Tasks;
using Xunit;

namespace Automation.TestFramework.Tests
{
    [TestCase(nameof(AsyncLifetimeTest))]
    public class AsyncLifetimeTest : IAsyncLifetime, IDisposable
    {
        private bool _initializedAsync;
        private bool _disposedAsync;
        private int _value;

        public AsyncLifetimeTest()
        {
        }

        public void Dispose()
        {
            Assert.True(_initializedAsync);
            Assert.True(_disposedAsync);
            Assert.Equal(2, _value);
        }

        public Task InitializeAsync()
        {
            _initializedAsync = true;
            _value++;
            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            _disposedAsync = true;
            _value++;
            return Task.CompletedTask;
        }

        [Summary]
        public void Summary()
        {
        }

        [Input]
        private void Input1()
        {
        }

        [Input(2)]
        private void Input2()
        {
        }
    }
}
