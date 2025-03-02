using System;
using System.Collections.Generic;
using Xunit;

namespace Automation.TestFramework.Dynamic.Tests.SourceGenerators;

public partial class TestCase_Order : IDisposable
{
    public void Dispose()
    {
        Assert.Equal(4, CallOrder.Count);
        Assert.Equal([
                "Input1",
                "ExpectedResult1",
                "Input2",
                "ExpectedResult2"
            ],
            CallOrder);
    }

    private List<string> CallOrder { get; } = [];

    [Summary]
    public partial void Summary();

    [Input(2)]
    public void Input2()
    {
        CallOrder.Add(nameof(Input2));
    }
    
    [Input(1)]
    public void Input1()
    {
        CallOrder.Add(nameof(Input1));
    }
    
    [ExpectedResult(1)]
    public void ExpectedResult1()
    {
        CallOrder.Add(nameof(ExpectedResult1));
    }
    
    [ExpectedResult(2)]
    public void ExpectedResult2()
    {
        CallOrder.Add(nameof(ExpectedResult2));
    }
}