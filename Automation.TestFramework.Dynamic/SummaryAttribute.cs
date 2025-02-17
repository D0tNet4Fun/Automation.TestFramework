using Automation.TestFramework.Dynamic.Discovery;
using Xunit;
using Xunit.v3;

namespace Automation.TestFramework.Dynamic;

/// <summary>
/// Identifies a method as the summary of a test case. This is the test case entry point, from which the test case steps are run in order. 
/// </summary>
[XunitTestCaseDiscoverer(typeof(TestCaseDiscoverer))]
public class SummaryAttribute : FactAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SummaryAttribute"/> class.
    /// </summary>
    /// <param name="description">The user-friendly description of the target test method.</param>
    public SummaryAttribute(string? description = null)
    {
        DisplayName = description;
    }

    /// <summary>
    /// Gets or sets the display name. There's no need to set this if the description was provided.
    /// </summary>
    public new string? DisplayName
    {
        get => base.DisplayName;
        set => base.DisplayName = value;
    }
}