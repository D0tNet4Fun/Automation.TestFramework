using System.Threading.Tasks;
using Automation.TestFramework.Dynamic.ObjectModel;
using Xunit;

namespace Automation.TestFramework.Dynamic.Tests;

public class TestCaseWithExpectedResult
{
    private int _value;

    [Summary("Test case with expected result with one assertion")]
    public void Summary()
    {
        TestCase.Current.Descriptor
            .AddStep(StepType.Input, "Input", Input)
            .AddAsyncStep(StepType.ExpectedResult, "Expected result", ExpectedResult);
    }

    private void Input()
    {
        _value = 1;
    }

    private async Task ExpectedResult()
    {
        Assert.Equal(1, _value);

        _value = 2;
        await Task.Delay(100);
        Step.Current.GetDescriptor<IExpectedResultDescriptor>()
            .Assert("This should work", () =>
            {
                Assert.Equal(2, _value);
                _value = 3;
            });
        
        Assert.Equal(3, _value);
    }
}