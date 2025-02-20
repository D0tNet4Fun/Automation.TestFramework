using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Automation.TestFramework.Dynamic.ObjectModel;

// ReSharper disable once CheckNamespace
namespace Automation.TestFramework;

public class TestCase : ITestCase
{
    private static readonly AsyncLocal<ITestCase?> Local = new();
    
    private readonly List<Step> _steps = [];

    private TestCase()
    {
        
    }

    public static ITestCase Current => Local.Value ?? throw new InvalidOperationException("Current test case is null");

    public ITestCase AddStep(StepType stepType, string description, Action code) =>
        AddStepFromDelegate(stepType, description, code);

    public ITestCase AddAsyncStep(StepType stepType, string description, Func<Task> code) =>
        AddStepFromDelegate(stepType, description, code);

    private ITestCase AddStepFromDelegate(StepType stepType, string description, Delegate code)
    {
        var order = stepType switch
        {
            StepType.ExpectedResult => _steps.Count(s => s.Type == StepType.Input),
            _ => _steps.Count(s => s.Type == stepType) + 1,
        };
        var step = new Step(stepType, order, description, code);
        _steps.Add(step);
        
        return this;
    }

    internal IReadOnlyCollection<Step> GetSteps() => _steps.AsReadOnly();
    
    internal static void SetCurrent() => Local.Value = new TestCase();
    internal static void ResetCurrent() => Local.Value = null;
}