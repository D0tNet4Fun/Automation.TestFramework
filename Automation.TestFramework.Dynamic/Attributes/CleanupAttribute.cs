namespace Automation.TestFramework;

/// <summary>
/// Identifies a method as a cleanup step of a test case.
/// Cleanup steps run after all the other steps.
/// </summary>
/// <param name="order">The order in which this cleanup step needs to be executed. This is relative to other cleanup steps.</param>
/// <param name="description">The user-friendly description of the cleanup step.</param>
public sealed class CleanupAttribute(int order = StepAttribute.DefaultOrder, string? description = null)
    : StepAttribute(StepType.Cleanup, order, description);