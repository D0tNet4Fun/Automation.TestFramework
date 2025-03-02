using Automation.TestFramework.Dynamic.ObjectModel;

// ReSharper disable once CheckNamespace
namespace Automation.TestFramework;

/// <summary>
/// Provided access to the descriptor used to define a test case from steps. 
/// </summary>
public interface IDynamicTestCase
{
    /// <summary>
    /// Gets the test case descriptor.
    /// </summary>
    ITestCaseDescriptor Descriptor { get; }
}