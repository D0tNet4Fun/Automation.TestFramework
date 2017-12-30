using System;
using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace Automation.TestFramework.Tests
{
    public class ThisClassIsNotATestCase
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ThisClassIsNotATestCase(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Test1()
        {
            // do something really bad to prove this test was not run
            _testOutputHelper.WriteLine("Just about to Crash the test runner");
            Process.GetCurrentProcess().Kill();
        }
    }
}
