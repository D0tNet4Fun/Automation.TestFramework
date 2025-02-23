using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Automation.TestFramework.Dynamic.Runners;
using Xunit.v3;

namespace Automation.TestFramework.Dynamic.ObjectModel;

internal class StepDescriptor : IStepDescriptor
{
    private readonly List<SubStep> _subSteps = [];

    public int SubStepCount { get; private set; }
    
    public IReadOnlyCollection<SubStep> GetSubSteps()
    {
        try
        {
            return _subSteps.ToArray();
        }
        finally
        {
            _subSteps.Clear();
        }
    }

    public IStepDescriptor ExecuteSubStep(SubStepType type, string description, Action code)
    {
        AddSubStepFromDelegate(type, description, code);
        Execute();

        return this;
    }

    public IStepDescriptor ExecuteAsyncSubStep(SubStepType type, string description, Func<Task> code)
    {
        AddSubStepFromDelegate(type, description, code);
        Execute();

        return this;
    }

    private IStepDescriptor AddSubStepFromDelegate(SubStepType type, string description, Delegate code)
    {
        var order = SubStepCount + 1;
        var subStep = new SubStep(type, order, description, code);
        _subSteps.Add(subStep);
        SubStepCount++;

        return this;
    }

    private void Execute()
    {
        var task = StepRunner.Instance.RunCurrentStepSubSteps();
        task.GetAwaiter().GetResult();
    }
}