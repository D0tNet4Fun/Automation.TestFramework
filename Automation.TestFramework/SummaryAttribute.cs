using System;
using Xunit;
using Xunit.Sdk;

namespace Automation.TestFramework
{
    /// <summary>
    /// Identifies a test method as the summary of a test case.
    /// </summary>
    [XunitTestCaseDiscoverer("Automation.TestFramework.Discovery.TestCaseDiscoverer", "Automation.TestFramework")]
    [AttributeUsage(AttributeTargets.Method)]
    public class SummaryAttribute : Attribute
    {
        private readonly string _description;

        public SummaryAttribute(string description)
        {
            if (string.IsNullOrEmpty(description))
                throw new ArgumentException("Description cannot be null or empty.", nameof(description));

            _description = description;
        }

        public string DisplayName
        {
            get => _description;
            set => throw new NotSupportedException("Overridden by constructor");
        }
    }
}