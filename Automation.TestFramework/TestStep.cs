using System;
using System.Threading;
using Automation.TestFramework.Entities;

namespace Automation.TestFramework
{
    /// <summary>
    /// Contains information about a test step.
    /// </summary>
    public class TestStep
    {
        private static AsyncLocal<TestStep> _current = new AsyncLocal<TestStep>();

        private TestStep()
        {
        }

        /// <summary>
        /// Gets the current test step of the current test case.
        /// </summary>
        public static TestStep Current => _current.Value ?? throw new InvalidOperationException("The current test step could not be determined.");

        /// <summary>
        /// Gets the expected result.
        /// </summary>
        public IExpectedResult ExpectedResult { get; internal set; }

        internal static TestStep SetCurrent()
        {
            var testStep = new TestStep();
            _current.Value = testStep;
            return testStep;
        }

        /// <summary>
        /// Use <see cref="Current"/> instead.
        /// </summary>
        public static TestStep GetCurrent() => Current;

        internal static void ResetCurrent()
        {
            _current.Value = null;
        }
    }
}