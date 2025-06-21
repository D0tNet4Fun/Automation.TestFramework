using System;
using System.Threading.Tasks;
using Xunit;

namespace Automation.TestFramework.Dynamic.Tests;

public class TestCase_Input_SubSteps : IDisposable
{
    private int _value;

    public void Dispose()
    {
        Assert.Equal(6, _value);
    }

    [Summary("Input with multiple sub-steps")]
    public void Summary()
    {
        TestCase.Current.Descriptor
            .AddAsyncStep(StepType.Input, "Input", Input);
    }

    private async Task Input()
    {
        _value = 1;
        Step.Current.Descriptor
            .AddSubStep("Phase 1", () => { _value = 2; })
            .AddSubStep("Phase 2", () => { _value = 3; })
            .Execute();

        await Task.Delay(100);

        Step.Current.Descriptor
            .ExecuteSubStep("Phase 3", () => { _value = 4; })
            .AddSubStep("Phase 4", () =>
            {
                Assert.Equal(4, _value);
                _value = 5;
            })
            .AddSubStep("Phase 5", () =>
            {
                Assert.Equal(5, _value);
                _value = 6;
            });
        // phases 4 and 5 are not executed explicitly,
        // however they will be executed by the framework as if the user called Execute here.
    }
}