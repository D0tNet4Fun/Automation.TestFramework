// ReSharper disable once CheckNamespace
namespace Automation.TestFramework;

/// <summary>
/// Defines the type of a test case step.
/// </summary>
public enum StepType
{
    None,

    /// <summary>
    /// Setup.
    /// </summary>
    Setup,

    /// <summary>
    /// Precondition.
    /// </summary>
    Precondition,

    /// <summary>
    /// Input.
    /// </summary>
    Input,

    /// <summary>
    /// Expected result.
    /// </summary>
    ExpectedResult,

    /// <summary>
    /// Cleanup.
    /// </summary>
    Cleanup
}