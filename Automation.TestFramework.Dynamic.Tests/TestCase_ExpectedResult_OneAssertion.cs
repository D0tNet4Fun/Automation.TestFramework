using System;
using Xunit;

namespace Automation.TestFramework.Dynamic.Tests;

public class TestCase_ExpectedResult_OneAssertion : IDisposable
{
    private int _value;

    public void Dispose()
    {
        Assert.Equal(3, _value);
    }

    [Summary("Expected result with one assertion")]
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

        _value = 2;
        ExpectedResultStep.Current.Descriptor
            .Assert("This should work", () =>
            {
                Assert.Equal(2, _value);
                _value = 3;
            });
    }
}