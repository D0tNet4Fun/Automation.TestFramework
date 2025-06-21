using Automation.TestFramework.Dynamic.ObjectModel;

// ReSharper disable once CheckNamespace
namespace Automation.TestFramework;

/// <summary>
/// Provides access to descriptors used to define a step from sub-steps.
/// </summary>
public interface IDynamicStep
{
    /// <summary>
    /// Gets the generic descriptor. 
    /// </summary>
    IStepDescriptor Descriptor { get; }
    
    /// <summary>
    /// Gets a custom descriptor.
    /// </summary>
    /// <typeparam name="TStepDescriptor">The type of the descriptor.</typeparam>
    /// <returns></returns>
    TStepDescriptor GetDescriptor<TStepDescriptor>() 
        where TStepDescriptor : IStepDescriptor;
}