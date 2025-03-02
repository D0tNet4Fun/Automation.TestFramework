using Xunit;

namespace Automation.TestFramework.Dynamic.Tests;

[TestCase("ignore this")]
public class TestCase_BackwardCompatibility
{
    [Summary]
    public void Summary()
    {
        TestCase.Current.Descriptor
            .AddStep(StepType.Input, "This is the input", () => { })
            .AddStep(StepType.ExpectedResult, "This is the expected result", () =>
            {
                TestStep.Current.ExpectedResult
                    .Assert("This always works", () => Assert.True(true));
            });
    }
}