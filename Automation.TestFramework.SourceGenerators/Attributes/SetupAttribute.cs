using Automation.TestFramework.SourceGenerators.ObjectModel;

// ReSharper disable once CheckNamespace
namespace Automation.TestFramework;

/// <summary>
/// Identifies a method as a setup step of a test case.
/// Setup steps run before all the other steps.
/// </summary>
/// <param name="order">The order in which this setup step needs to be executed. This is relative to other setup steps.</param>
/// <param name="description">The user-friendly description of the setup step.</param>
public sealed class SetupAttribute(int order = StepAttribute.DefaultOrder, string? description = null)
    : StepAttribute(StepType.Setup, order, description);