using System;

namespace Automation.TestFramework.Dynamic.ObjectModel;

internal class ExpectedResult(int index, int order, string description, Delegate code) : 
    Step(StepType.ExpectedResult, index, order, description, code), 
    IExpectedResult
{
    public IExpectedResult Assert(string description, Action action)
    {
        return (IExpectedResult)ExecuteSubStep(SubStepType.Assertion, description, action);
    }
}