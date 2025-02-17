using System.Threading.Tasks;
using Automation.TestFramework.Dynamic.Entities;
using Xunit;

namespace Automation.TestFramework.Dynamic.Tests;

public class BasicTest
{
    private int _value;

    [Summary("Basic test case")]
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