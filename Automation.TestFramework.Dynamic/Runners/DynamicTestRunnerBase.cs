using System;
using System.Threading.Tasks;
using Automation.TestFramework.Dynamic.ObjectModel;
using Xunit.v3;

namespace Automation.TestFramework.Dynamic.Runners;

/// <summary>
/// The base test runner for dynamic tests (with overridable context).
/// </summary>
internal class DynamicTestRunnerBase<TContext> : XunitTestRunnerBase<TContext, IDynamicTest> 
    where TContext : XunitTestRunnerBaseContext<IDynamicTest>
{
    protected override bool IsTestClassCreatable(TContext ctxt)
    {
        // dynamic tests are based on delegates. they do not need to create test class instances.   
        return false;
    }

    protected override ValueTask<TimeSpan> InvokeTest(TContext ctxt, object? testClassInstance)
    {
        // invoke the test using the dynamic test's target, instead of the provided test class instance (which is null, because the test runner does not create it)
        var target = ctxt.Test.Target;
        return base.InvokeTest(ctxt, target);
    }

    protected override bool IsTestClassDisposable(TContext ctxt, object testClassInstance)
    {
        // dynamic tests are based on delegates
        return false;
    }
}