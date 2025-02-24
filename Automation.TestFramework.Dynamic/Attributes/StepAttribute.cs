using System;

namespace Automation.TestFramework;

[AttributeUsage(AttributeTargets.Method)]
public class StepAttribute(StepType type, int order = StepAttribute.DefaultOrder, string? description = null) : Attribute
{
    // ReSharper disable once MemberCanBeProtected.Global
    public const int DefaultOrder = 1;

    public StepType Type { get; } = type;
    public int Order { get; } = order;
    public string? Description { get; } = description;
}