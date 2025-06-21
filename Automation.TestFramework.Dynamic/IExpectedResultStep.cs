using Automation.TestFramework.Dynamic.ObjectModel;

// ReSharper disable once CheckNamespace
namespace Automation.TestFramework;

public interface IExpectedResultStep : IDynamicStep
{
    /// <summary>
    /// Gets the expected result descriptor.
    /// </summary>
    new IExpectedResultDescriptor Descriptor { get; }
}