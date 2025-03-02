using System;
using Xunit;

namespace Automation.TestFramework.Dynamic.Tests;

public class TestCase_Error_Cleanup : IDisposable
{
    private bool _calledCleanup;

    public void Dispose()
    {
        Assert.True(_calledCleanup);
    }

    [Summary("Error in one step skips the next steps, except cleanup")]
    public void Summary()
    {
        TestCase.Current.Descriptor
            .AddStep(StepType.Input, "This is the input", Input)
            .AddStep(StepType.ExpectedResult, "This is the expected result", ExpectedResult)
            .AddStep(StepType.Cleanup, "This is the mandatory cleanup", Cleanup);
    }

    private void Input()
    {
        Assert.Fail("Input failed on purpose");
    }

    private void ExpectedResult()
    {
    }

    private void Cleanup()
    {
        _calledCleanup = true;
    }
}