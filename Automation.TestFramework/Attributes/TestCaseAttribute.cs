using System;
using System.Runtime.CompilerServices;

namespace Automation.TestFramework
{
    /// <summary>
    /// Identifies a test class as a test case.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class TestCaseAttribute : Attribute
    {
        /// <summary>
        /// Gets the ID of the test case.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the source of the test case.
        /// </summary>
        public string Source { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseAttribute" /> class.
        /// </summary>
        /// <param name="id">The ID of the test case.</param>
        public TestCaseAttribute(string id, [CallerFilePath] string source = null)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id), "The ID is not set");

            Id = id;
            Source = source;
        }
    }
}
