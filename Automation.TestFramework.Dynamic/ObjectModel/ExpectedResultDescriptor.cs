using System;
using System.Threading.Tasks;

namespace Automation.TestFramework.Dynamic.ObjectModel;

internal class ExpectedResultDescriptor : StepDescriptor, IExpectedResultDescriptor
{
    public IExpectedResultDescriptor Assert(string description, Action code) => 
        (IExpectedResultDescriptor)ExecuteSubStep(SubStepType.Assertion, description, code);

    public IExpectedResultDescriptor AssertAsync(string description, Func<Task> code) =>
        (IExpectedResultDescriptor)ExecuteAsyncSubStep(SubStepType.Assertion, description, code);

    public IExpectedResultDescriptor Verify(string description, Action code) => 
        (IExpectedResultDescriptor)ExecuteSubStep(SubStepType.Verification, description, code);

    public IExpectedResultDescriptor VerifyAsync(string description, Func<Task> code) =>
        (IExpectedResultDescriptor)ExecuteAsyncSubStep(SubStepType.Verification, description, code);
}