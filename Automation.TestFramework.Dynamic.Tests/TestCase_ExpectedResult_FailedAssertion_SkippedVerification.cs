using System;
using Xunit;

namespace Automation.TestFramework.Dynamic.Tests;

public class TestCase_ExpectedResult_FailedAssertion_SkippedVerification : IDisposable
{
    private int _value;
    private bool _calledVerification;

    public void Dispose()
    {
        Assert.False(_calledVerification);
    }

    [Summary("Expected result with one failed assertion and one skipped verification")]
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

        ExpectedResultStep.Current.Descriptor
            .Assert("This should fail", () =>
            {
                Assert.Fail("Failed on purpose");
            })
            .Verify("This should be skipped", () =>
            {
                _calledVerification = true;
            });
    }
}