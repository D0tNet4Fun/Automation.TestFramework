namespace Automation.TestFramework;

/// <summary>
/// Identifies a method as a precondition step of a test case.
/// Precondition steps run after setups and before inputs.
/// </summary>
/// <param name="order">The order in which this precondition step needs to be executed. This is relative to other precondition steps.</param>
/// <param name="description">The user-friendly description of the precondition step.</param>
public sealed class PreconditionAttribute(int order = StepAttribute.DefaultOrder, string? description = null)
    : StepAttribute(StepType.Precondition, order, description);