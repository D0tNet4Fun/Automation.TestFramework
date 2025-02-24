using System;
using Xunit;

namespace Automation.TestFramework.Dynamic.Tests.SourceGenerators;

public partial class TestCase_Error_Cleanup : IDisposable
{
    private bool _calledCleanup;

    public void Dispose()
    {
        Assert.True(_calledCleanup);
    }

    [Summary("Error in one step skips the next steps, except cleanup - generated")]
    public partial void Summary();

    [Input(1, "This is the input")]
    private void Input()
    {
        Assert.Fail("Input failed on purpose");
    }

    [ExpectedResult(1, "This is the expected result")]
    private void ExpectedResult()
    {
    }

    [Cleanup(1, "This is the mandatory cleanup")]
    private void Cleanup()
    {
        _calledCleanup = true;
    }
}