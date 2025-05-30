namespace Automation.TestFramework.Dynamic.Tests;

public class TestCase_MoreThan10Steps
{
    [Summary("Test case with more than 10 steps - check order")]
    public void Summary()
    {
        var descriptor = TestCase.Current.Descriptor;

        for (var i = 0; i < 20; i++)
        {
            descriptor.AddStep(StepType.Input, $"Input {i + 1}", () => { });
        }
    }
}