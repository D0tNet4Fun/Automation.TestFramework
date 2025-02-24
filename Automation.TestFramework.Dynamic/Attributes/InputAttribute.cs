namespace Automation.TestFramework;

public sealed class InputAttribute(int order = StepAttribute.DefaultOrder, string? description = null)
    : StepAttribute(StepType.Input, order, description);