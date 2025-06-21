using Automation.TestFramework.Dynamic;

// ReSharper disable once CheckNamespace
namespace Automation.TestFramework;

/// <summary>
/// Provides access to the current test case.
/// </summary>
public abstract class TestCase : AsyncLocalContainer<IDynamicTestCase>;