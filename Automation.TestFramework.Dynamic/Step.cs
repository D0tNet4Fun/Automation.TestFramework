using Automation.TestFramework.Dynamic;

// ReSharper disable once CheckNamespace
namespace Automation.TestFramework;

/// <summary>
/// Provides access to the current step.
/// </summary>
public abstract class Step : AsyncLocalContainer<IDynamicStep>;