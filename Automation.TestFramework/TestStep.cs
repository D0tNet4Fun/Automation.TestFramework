using System;

namespace Automation.TestFramework
{
    /// <summary>
    /// Contains information about a test step.
    /// </summary>
    public class TestStep
    {
        [ThreadStatic]
        private static TestStep _current;

        private TestStep()
        {
        }

        /// <summary>
        /// Gets the current test step of the current test case.
        /// </summary>
        public static TestStep Current => _current;

        internal static void InitializeCurrent() => _current = new TestStep();

        /// <summary>
        /// Gets the expected result.
        /// </summary>
        public IExpectedResult ExpectedResult { get; internal set; }
    }
}