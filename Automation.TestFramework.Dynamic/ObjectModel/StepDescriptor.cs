using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Automation.TestFramework.Dynamic.ObjectModel;

internal class StepDescriptor(Step step) : IStepDescriptor
{
    private readonly List<SubStep> _subSteps = [];
    public int ExecutedSubStepCount { get; private set; }

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
        var order = _subSteps.Count + 1;
        var subStep = new SubStep(type, order, description, code);
        _subSteps.Add(subStep);

        return this;
    }

    public void Execute()
    {
        // get the sub steps added since the previous execution
        var subSteps = _subSteps.Skip(ExecutedSubStepCount).ToArray();
        if (subSteps.Length == 0) return;

        try
        {
            step.Execute(subSteps);
        }
        finally
        {
            // make sure the just executed steps will not be included in the next execution
            ExecutedSubStepCount += subSteps.Length;
        }
    }
}