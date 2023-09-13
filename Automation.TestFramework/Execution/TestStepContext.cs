using Automation.TestFramework.Entities;
using System;

namespace Automation.TestFramework.Execution
{
    internal class TestStepContext : IDisposable
    {
        private readonly ExpectedResult _expectedResult;

        public TestStep TestStep { get; private set; }

        public TestStepContext(ExpectedResult expectedResult)
        {
            _expectedResult = expectedResult;
        }

        /// <summary>
        /// Called by the test invoker before the test method is invoked, on the thread on which the test method will be invoked.
        /// </summary>
        public void Initialize()
        {
            TestStep = TestStep.SetCurrent();
            TestStep.ExpectedResult = _expectedResult;
        }

        public void Dispose()
        {
            TestStep.ResetCurrent();
            TestStep = null;
        }
    }
}
