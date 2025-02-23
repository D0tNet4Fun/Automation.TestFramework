using System;
using Automation.TestFramework.Dynamic.ObjectModel;
using Xunit;

namespace Automation.TestFramework.Dynamic.Tests;

public class TestCase_ExpectedResult_FailedVerfication_FailedAssertion : IDisposable
{
    private int _value;
    private bool _calledAssertion;

    public void Dispose()
    {
        Assert.True(_calledAssertion);
    }

    [Summary("Expected result with one failed verification and one failed assertion")]
    public void Summary()
    {
        TestCase.Current.Descriptor
            .AddStep(StepType.Input, "Input", Input)
            .AddStep(StepType.ExpectedResult, "Expected result", ExpectedResult);
    }

    private void Input()
    {
        _value = 1;
    }

    private void ExpectedResult()
    {
        Assert.Equal(1, _value);

        Step.Current.GetDescriptor<IExpectedResultDescriptor>()
            .Verify("This should fail, but it's not critical", () =>
            {
                Assert.Fail("Failed verification on purpose");
            })
            .Assert("This should fail", () =>
            {
                _calledAssertion = true;
                Assert.Fail("Failed assertion on purpose");
            });
    }
}