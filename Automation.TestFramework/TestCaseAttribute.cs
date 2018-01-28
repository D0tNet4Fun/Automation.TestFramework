using System;

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
        /// Initializes a new instance of the <see cref="TestCaseAttribute" /> class.
        /// </summary>
        /// <param name="id">The ID of the test case.</param>
        public TestCaseAttribute(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id), "The ID is not set");

            Id = id;
        }
    }
}
