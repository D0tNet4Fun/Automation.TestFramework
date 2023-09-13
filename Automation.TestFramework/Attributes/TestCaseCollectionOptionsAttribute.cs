using System;

namespace Automation.TestFramework
{
    /// <summary>
    /// Defines options for test case collections.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class TestCaseCollectionOptionsAttribute : Attribute
    {
        /// <summary>
        /// Determines how to execute the test cases in this collection.
        /// </summary>
        public TestCaseExecutionMode ExecutionMode { get; set; }
    }

    /// <summary>
    /// Defines how to execute test cases inside a collection.
    /// </summary>
    public enum TestCaseExecutionMode
    {
        /// <summary>
        /// Test cases are executed in parallel, unless parallelization is disabled.
        /// Use when the test cases inside a collection have a shared context but they can be executed in isolation.
        /// </summary>
        /// <remarks>
        /// This limits the role of a collection to be merely a shared context provider, unlike in xunit.
        /// </remarks>
        InParallelIfAllowed,
        /// <summary>
        /// Test cases are executed sequentially.
        /// </summary>
        Sequential
    }
}