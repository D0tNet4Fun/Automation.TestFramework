// this file provides compatibility guidelines after upgrading a project from Automation.TestFramework to Automation.TestFramework.Dynamic

using System;
using Automation.TestFramework.Dynamic.ObjectModel;

// ReSharper disable once CheckNamespace
namespace Automation.TestFramework;

[Obsolete("Use " + nameof(Step) + " instead.")]
public class TestStep
{
    private TestStep()
    {
    }
    
    public static TestStep Current { get; } = new();

    [Obsolete("Use " + nameof(Current) + " instead.")]
    public static TestStep GetCurrent() => Current;

    [Obsolete("Use " + nameof(ExpectedResultStep) + "." + nameof(ExpectedResultStep.Current) + "." + nameof(ExpectedResultStep.Current.Descriptor) +" instead.")]
    public IExpectedResultDescriptor ExpectedResult => ExpectedResultStep.Current.Descriptor;
}