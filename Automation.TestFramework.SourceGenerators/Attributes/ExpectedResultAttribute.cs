using Automation.TestFramework.SourceGenerators.ObjectModel;

// ReSharper disable once CheckNamespace
namespace Automation.TestFramework;

/// <summary>
/// Identifies a method as an expected result step of a test case.
/// Expected result steps run after same order input setups. They can be followed by other input steps with a higher order.
/// </summary>
/// <param name="order">The order in which this expected result step needs to be executed. This must match the order of the preceding input step.</param>
/// <param name="description">The user-friendly description of the expected result step.</param>
public sealed class ExpectedResultAttribute(int order = StepAttribute.DefaultOrder, string? description = null)
    : StepAttribute(StepType.ExpectedResult, order, description);