using System;
using Xunit;

namespace Automation.TestFramework.Dynamic.Tests;

public class TestCase_Error_SkipsOtherSteps : IDisposable
{
    private bool _calledExpectedResult;

    public void Dispose()
    {
        Assert.False(_calledExpectedResult);
    }

    [Summary("Error in one step skips the next steps")]
    public void Summary()
    {
        TestCase.Current.Descriptor
            .AddStep(StepType.Input, "This is the 1st input", Input)
            .AddStep(StepType.ExpectedResult, "This is the expected result", ExpectedResult)
            .AddStep(StepType.Input, "This is the 2nd input", Input2);
    }

    private void Input()
    {
        Assert.Fail("Input failed on purpose");
    }

    private void ExpectedResult()
    {
        _calledExpectedResult = true;
    }
    
    private void Input2()
    {
        Assert.Fail("Input 2 failed on purpose");
    }
}