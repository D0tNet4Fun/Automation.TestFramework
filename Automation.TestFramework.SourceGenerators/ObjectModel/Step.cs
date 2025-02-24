namespace Automation.TestFramework.SourceGenerators.ObjectModel;

public class Step(StepType type, int order, string description, string methodName, bool isAsync)
{
    public StepType Type { get; } = type;
    public int Order { get; } = order;
    public string Description { get; } = description;
    public string MethodName { get; } = methodName;
    public bool IsAsync { get; } = isAsync;
}