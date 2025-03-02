using System;
using System.Threading.Tasks;
using Automation.TestFramework.Dynamic.ObjectModel;
using Xunit;

namespace Automation.TestFramework.Dynamic.Tests;

public class TestCase_ExpectedResult_MultipleAssertions : IDisposable
{
    private int _value;

    public void Dispose()
    {
        Assert.Equal(4, _value);
    }

    [Summary("Expected result with multiple assertions")]
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
        Step.Current.GetDescriptor<IExpectedResultDescriptor>()
            .Assert("This should work", () =>
            {
                Assert.Equal(2, _value);
                _value = 3;
            })
            .AssertAsync("This should work too", async () =>
            {
                await Task.Delay(100);
                Assert.Equal(3, _value);
                _value = 4;
            });
    }
}