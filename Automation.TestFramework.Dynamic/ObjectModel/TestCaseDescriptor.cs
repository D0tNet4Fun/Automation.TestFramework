using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Automation.TestFramework.Dynamic.ObjectModel;

internal class TestCaseDescriptor:  ITestCaseDescriptor
{
    private readonly List<Step> _steps = [];
    
    public int StepCount { get; private set; }

    public ITestCaseDescriptor AddStep(StepType stepType, string description, Action code) =>
        AddStepFromDelegate(stepType, description, code);

    public ITestCaseDescriptor AddAsyncStep(StepType stepType, string description, Func<Task> code) =>
        AddStepFromDelegate(stepType, description, code);

    public ITestCaseDescriptor AddAsyncStep(StepType stepType, string description, Func<ValueTask> code) =>
        AddStepFromDelegate(stepType, description, code);

    private ITestCaseDescriptor AddStepFromDelegate(StepType stepType, string description, Delegate code)
    {
        var index = _steps.Count + 1;
        
        var order = stepType switch
        {
            StepType.ExpectedResult => _steps.Count(s => s.Type == StepType.Input),
            _ => _steps.Count(s => s.Type == stepType) + 1,
        };

        StepDescriptor SelectDescriptor(Step step)
        {
            return stepType switch
            {
                StepType.ExpectedResult => new ExpectedResultDescriptor(step),
                _ => new StepDescriptor(step)
            };
        }

        var step = new Step(stepType, index, order, description, code, SelectDescriptor);
        _steps.Add(step);

        StepCount++;

        return this;
    }

    public IReadOnlyCollection<Step> GetSteps()
    {
        try
        {
            return _steps.ToArray();
        }
        finally
        {
            _steps.Clear();
        }
    }
}