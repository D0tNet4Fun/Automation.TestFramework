using System.Threading.Tasks;
using Xunit;

namespace Automation.TestFramework.Dynamic.Tests;

public class TestCase_AsyncSteps
{
    private int _value;

    [Summary("Test case with async steps")]
    public void Summary()
    {
        TestCase.Current.Descriptor
            .AddAsyncStep(StepType.Input, "This is the 1st input", Input)
            .AddAsyncStep(StepType.ExpectedResult, "This is the 1st expected result", ExpectedResult)
            .AddAsyncStep(StepType.Input, "This is the 2nd input", Input2)
            .AddAsyncStep(StepType.ExpectedResult, "This is the 2nd expected result", ExpectedResult2)
            .AddAsyncStep(StepType.Input, "This is the 3rd input", Input3)
            .AddAsyncStep(StepType.ExpectedResult, "This is the 3rd expected result", ExpectedResult3)
            ;
    }

    private async Task Input()
    {
        await Task.Delay(10);
        _value++;
    }

    private async Task ExpectedResult()
    {
        await Task.Delay(10);
        Assert.Equal(1, _value);
    }
    
    private Task Input2()
    {
        // no await here
        _value++;
        return Task.CompletedTask;
    }
    
    private ValueTask ExpectedResult2()
    {
        // no await
        Assert.Equal(2, _value);
        return ValueTask.CompletedTask;
    }

    private async ValueTask Input3()
    {
        await Task.Delay(10);
        _value++;
    }

    private async ValueTask ExpectedResult3()
    {
        await Task.Delay(10);
        Assert.Equal(3, _value);
    }
}