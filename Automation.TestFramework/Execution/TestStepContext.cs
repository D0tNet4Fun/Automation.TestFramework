using Automation.TestFramework.Entities;
using System;

namespace Automation.TestFramework.Execution
{
    internal class TestStepContext : IDisposable
    {
        private readonly string _source;
        private readonly ExpectedResult _expectedResult;

        public TestStep TestStep { get; private set; }

        public TestStepContext(string source, ExpectedResult expectedResult)
        {
            _source = source;
            _expectedResult = expectedResult;
        }

        /// <summary>
        /// Called by the test invoker before the test method is invoked, on the thread on which the test method will be invoked.
        /// </summary>
        public void Initialize()
        {
            TestStep = TestStep.InitializeCurrent(_source);
            TestStep.ExpectedResult = _expectedResult;
        }

        public void Dispose()
        {
            TestStep.RemoveCurrent(_source);
            TestStep = null;
        }
    }
}
