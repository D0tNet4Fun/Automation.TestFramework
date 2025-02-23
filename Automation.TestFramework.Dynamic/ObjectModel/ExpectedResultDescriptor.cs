using System;
using System.Threading.Tasks;

namespace Automation.TestFramework.Dynamic.ObjectModel;

internal class ExpectedResultDescriptor : StepDescriptor, IExpectedResultDescriptor
{
    public IExpectedResultDescriptor Assert(string description, Action code) => 
        (IExpectedResultDescriptor)ExecuteSubStep(SubStepType.Assertion, description, code);

    public IExpectedResultDescriptor AssertAsync(string description, Func<Task> code) =>
        (IExpectedResultDescriptor)ExecuteAsyncSubStep(SubStepType.Assertion, description, code);
}