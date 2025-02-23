using System;

namespace Automation.TestFramework.Dynamic.ObjectModel;

internal class ExpectedResultDescriptor : StepDescriptor, IExpectedResultDescriptor
{
    public IExpectedResultDescriptor Assert(string description, Action code) => 
        (IExpectedResultDescriptor)ExecuteSubStep(SubStepType.Assertion, description, code);
}