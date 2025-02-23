using System.Threading.Tasks;
using Automation.TestFramework.Dynamic.ObjectModel;
using Xunit;

namespace Automation.TestFramework.Dynamic.Tests;

public class TestCase_ExpectedResult_Verifications
{
    private int _value;

    [Summary("Expected result with multiple failed verifications")]
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
            .Verify("This should fail", () =>
            {
                Assert.Fail("Failed on purpose");
            })
            .VerifyAsync("This should fail too", async () =>
            {
                await Task.Delay(100);
                Assert.Fail("Failed on purpose");
            });
    }
}