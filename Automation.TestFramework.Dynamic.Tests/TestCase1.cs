using System.Threading.Tasks;
using Xunit;

namespace Automation.TestFramework.Dynamic.Tests;

public class TestCase1
{
    private int _value;

    [Summary("Test case 1 (POC)")]
    public void Summary()
    {
        TestCase.Current
            .AddStep(StepType.Input, "Input", Input)
            .AddAsyncStep(StepType.ExpectedResult, "Expected result", ExpectedResult);
    }

    private void Input()
    {
        _value = 1;
    }

    private async Task ExpectedResult()
    {
        await Task.Delay(100);
        Assert.Equal(1, _value);
    }
}