using System.Threading.Tasks;
using Xunit;

namespace Automation.TestFramework.Dynamic.Tests.SourceGenerators;

public partial class BasicTestCase
{
    private int _value;

    [Summary("Basic test case with input and expected result - generated")]
    public partial void Summary();

    [Input]
    private void Input()
    {
        _value = 1;
    }

    private async Task ExpectedResult()
    {
        await Task.Delay(100);
        Assert.Equal(1, _value);
    }
}