using System.Threading.Tasks;
using Xunit;

namespace Automation.TestFramework.Dynamic.Tests.SourceGenerators;

public partial class TestCase_AsyncSteps
{
    private int _value;

    [Summary("Test case with async steps")]
    public partial void Summary();

    [Input(1)]
    private async Task Input()
    {
        await Task.Delay(10);
        _value++;
    }

    [ExpectedResult(1)]
    private async Task ExpectedResult()
    {
        await Task.Delay(10);
        Assert.Equal(1, _value);
    }

    [Input(2)]
    private Task Input2()
    {
        // no await here
        _value++;
        return Task.CompletedTask;
    }

    [ExpectedResult(2)]
    private ValueTask ExpectedResult2()
    {
        // no await
        Assert.Equal(2, _value);
        return ValueTask.CompletedTask;
    }

    [Input(3)]
    private async ValueTask Input3()
    {
        await Task.Delay(10);
        _value++;
    }

    [ExpectedResult(3)]
    private async ValueTask ExpectedResult3()
    {
        await Task.Delay(10);
        Assert.Equal(3, _value);
    }
}