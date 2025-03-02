using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Automation.TestFramework.Dynamic.Tests.SourceGenerators;

public partial class TestCase_Hierarchy : TestCaseBase
{
    [Summary]
    public partial void Summary();

    [Input]
    public void Input()
    {
        CallOrder.Add(nameof(Input));
    }
}

public class TestCaseBase : IDisposable
{
    public void Dispose()
    {
        Assert.True(CallOrder.Count > 2);
        Assert.Equal("Setup", CallOrder.First());
        Assert.Equal("Cleanup", CallOrder.Last());
    }

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