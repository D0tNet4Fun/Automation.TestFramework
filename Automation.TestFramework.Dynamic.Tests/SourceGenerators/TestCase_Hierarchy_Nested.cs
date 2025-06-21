using System;
using System.Collections.Generic;
using Xunit;

namespace Automation.TestFramework.Dynamic.Tests.SourceGenerators;

public partial class TestCase_Hierarchy_Nested : TestCaseDerived, IDisposable
{
    public void Dispose()
    {
        Assert.Equal(7, CallOrder.Count);
        Assert.Equal([
            "SetupBase", 
            "SetupDerived1", "SetupDerived2", 
            "Input",
            "CleanupDerived1", "CleanupDerived2",
            "CleanupBase"], 
            CallOrder);
    }

    [Summary]
    public partial void Summary();

    [Input]
    public void Input()
    {
        CallOrder.Add(nameof(Input));
    }
}

public class TestCaseDerived : TestCaseBase2
{
    [Setup]
    public void SetupDerived1()
    {
        CallOrder.Add(nameof(SetupDerived1));
    }

    [Setup(2)]
    public void SetupDerived2()
    {
        CallOrder.Add(nameof(SetupDerived2));
    }

    [Cleanup]
    public void CleanupDerived1()
    {
        CallOrder.Add(nameof(CleanupDerived1));
    }

    [Cleanup(2)]
    public void CleanupDerived2()
    {
        CallOrder.Add(nameof(CleanupDerived2));
    }
}

public class TestCaseBase2
{
    public List<string> CallOrder { get; } = [];

    [Setup]
    protected void SetupBase()
    {
        CallOrder.Add(nameof(SetupBase));
    }

    [Cleanup]
    protected void CleanupBase()
    {
        CallOrder.Add(nameof(CleanupBase));
    }
}