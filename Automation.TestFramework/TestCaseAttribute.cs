using System;

namespace Automation.TestFramework
{
    /// <summary>
    /// Identifies a test class as a test case.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class TestCaseAttribute : Attribute
    {
        public string Id { get; }

        public TestCaseAttribute(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("The ID is not set");

            Id = id;
        }
    }
}
