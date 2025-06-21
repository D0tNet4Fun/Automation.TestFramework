using System;
using System.Collections.Generic;
using Xunit;

namespace Automation.TestFramework.Dynamic.Tests;

public class TestCase_ExpectedResult_FailedAssertion_SkippedOtherSubSteps : IDisposable
{
    private int _value;
    private readonly List<int> _calledSubSteps = []; 

    public void Dispose()
    {
        Assert.Empty(_calledSubSteps);
    }

    [Summary("Expected result with one failed assertion skips all the other sub-steps")]
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

        ExpectedResultStep.Current.Descriptor
            .Assert("This should fail", () =>
            {
                Assert.Fail("Failed on purpose");
            });
        
        ExpectedResultStep.Current.Descriptor
            .Assert("This should be skipped", () =>
            {
                _calledSubSteps.Add(1);
            });
        
        ExpectedResultStep.Current.Descriptor
            .Assert("This should be skipped too", () =>
            {
                _calledSubSteps.Add(2);
            });
    }
}