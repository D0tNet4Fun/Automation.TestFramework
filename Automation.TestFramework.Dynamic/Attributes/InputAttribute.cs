namespace Automation.TestFramework;

/// <summary>
/// Identifies a method as an input step of a test case.
/// Input steps run after setups and before cleanups.
/// </summary>
/// <param name="order">The order in which this input step needs to be executed. This is relative to other input steps.</param>
/// <param name="description">The user-friendly description of the input step.</param>
public sealed class InputAttribute(int order = StepAttribute.DefaultOrder, string? description = null)
    : StepAttribute(StepType.Input, order, description);