using System;

namespace Automation.TestFramework.Dynamic.ObjectModel;

public interface IStep
{
    IStep ExecuteSubStep(SubStepType type, string description, Action action);
}