using System;
using System.Threading;
using Xunit.Abstractions;

namespace Automation.TestFramework.Tests
{
    /// <summary>
    /// Base class for test case tests.
    /// </summary>
    public class TestCaseWorkerBase
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public TestCaseWorkerBase(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public void DoWork()
        {
            _testOutputHelper.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] Working for {GetType().Name}...");
            Thread.Sleep(TimeSpan.FromSeconds(5));
            _testOutputHelper.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] Work done for {GetType().Name}");
        }
    }
}