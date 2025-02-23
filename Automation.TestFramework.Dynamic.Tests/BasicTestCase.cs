using System.Threading.Tasks;
using Xunit;

namespace Automation.TestFramework.Dynamic.Tests;

public class BasicTestCase
{
    private int _value;

    [Summary("Basic test case with input and expected result")]
    public void Summary()
    {
        TestCase.Current.Descriptor
            .AddStep(StepType.Input, "This is the input", Input)
            .AddAsyncStep(StepType.ExpectedResult, "This is the expected result", ExpectedResult);
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