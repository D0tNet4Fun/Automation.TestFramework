using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Automation.TestFramework.Entities;

namespace Automation.TestFramework
{
    /// <summary>
    /// Contains information about a test step.
    /// </summary>
    public class TestStep
    {
        [ThreadStatic]
        private static TestStep _current;

        private static readonly ConcurrentDictionary<string, TestStep> CurrentTestSteps = new ConcurrentDictionary<string, TestStep>();

        private TestStep()
        {
        }

        /// <summary>
        /// Gets the current test step of the current test case.
        /// </summary>
        [Obsolete("Use " + nameof(GetCurrent) + "() instead which works correctly in async methods.")]
        public static TestStep Current => _current;

        /// <summary>
        /// Gets the expected result.
        /// </summary>
        public IExpectedResult ExpectedResult { get; internal set; }

        internal static TestStep InitializeCurrent(string source)
        {
            var currentTestStep = new TestStep();
            _current = currentTestStep;
            if (source != null && !CurrentTestSteps.TryAdd(source, currentTestStep))
            {
                throw new InvalidOperationException($"A test step has already been initialized for source {source}.");
            }
            return currentTestStep;
        }

        /// <summary>
        /// Gets the current test step of the current test case.
        /// </summary>
        public static TestStep GetCurrent([CallerFilePath] string sourceFilePath = null)
        {
            if (sourceFilePath == null) throw new ArgumentNullException(nameof(sourceFilePath)); // only if the compiler does not support [CallerFilePath]

            if (!CurrentTestSteps.TryGetValue(sourceFilePath, out var testStep))
            {
                throw new InvalidOperationException("The current test step could not be determined.");
            }
            return testStep;
        }

        internal static void RemoveCurrent(string source)
        {
            if (source != null && !CurrentTestSteps.TryRemove(source, out _))
            {
                throw new InvalidOperationException($"Could not remove the test step for source {source}.");
            }
        }
    }
}