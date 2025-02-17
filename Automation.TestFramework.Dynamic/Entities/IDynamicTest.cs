using Xunit.v3;

namespace Automation.TestFramework.Dynamic.Entities;

/// <summary>
/// Represents a test based on a delegate.
/// </summary>
public interface IDynamicTest : IXunitTest
{
    /// <summary>
    /// The delegate target.
    /// </summary>
    object? Target { get; }
    
    /// <summary>
    /// Gets or sets the skip reason.
    /// </summary>
    new string? SkipReason { get; set; }
}