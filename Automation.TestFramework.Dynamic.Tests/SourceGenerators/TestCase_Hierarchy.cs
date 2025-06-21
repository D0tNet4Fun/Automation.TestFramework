using System;
using System.Collections.Generic;
using Xunit;

namespace Automation.TestFramework.Dynamic.Tests.SourceGenerators;

public partial class TestCase_Hierarchy : TestCaseBase, IDisposable
{
    public void Dispose()
    {
        Assert.Equal(3, CallOrder.Count);
        Assert.Equal(["Setup", "Input", "Cleanup"], CallOrder);
    }

    [Summary]
    public partial void Summary();

    [Input]
    public void Input()
    {
        CallOrder.Add(nameof(Input));
    }
}

public class TestCaseBase
{
    public List<string> CallOrder { get; } = [];

    [Setup]
    protected void Setup()
    {
        CallOrder.Add(nameof(Setup));
    }

    [Cleanup]
    protected void Cleanup()
    {
        CallOrder.Add(nameof(Cleanup));
    }
}