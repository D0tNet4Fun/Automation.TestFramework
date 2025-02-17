using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Sdk;
using Xunit.v3;

namespace Automation.TestFramework.Dynamic.Entities;

internal class DynamicTest : IDynamicTest
{
    private static readonly Dictionary<string, IReadOnlyCollection<string>> NoTraits = new();
    private static readonly object?[] NoTestMethodArguments = [];
    
    public DynamicTest(
        IXunitTestCase testCase,
        string displayName,
        string uniqueID,
        int? timeout,
        Delegate @delegate)
    {
        if (@delegate.Method.GetParameters().Any())
        {
            throw new ArgumentException("The delegate must have a method without any parameters");
        }
        // if (@delegate.Target?.GetType() != testCase.TestClass.Class)
        // {
        //     throw new ArgumentException("The delegate target type must match the test class type");
        // }

        TestDisplayName = displayName;
        UniqueID = uniqueID;
        TestCase = testCase;
        TestMethod = new XunitTestMethod(testCase.TestClass, @delegate.Method, NoTestMethodArguments);
        Timeout = timeout ?? 0;
        Target = @delegate.Target;
    }

    public string TestDisplayName { get; }
    public IReadOnlyDictionary<string, IReadOnlyCollection<string>> Traits => NoTraits;
    public string UniqueID { get; }
    public bool Explicit => false;
    public string? SkipReason { get; set; }
    public IXunitTestCase TestCase { get; }
    public IXunitTestMethod TestMethod { get; }
    public object?[] TestMethodArguments => NoTestMethodArguments;
    public int Timeout { get; }
    Xunit.Sdk.ITestCase ITest.TestCase => TestCase;
    public object? Target { get; }
}