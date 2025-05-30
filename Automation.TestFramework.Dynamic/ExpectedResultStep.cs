using System;
using Automation.TestFramework.Dynamic;
using Automation.TestFramework.Dynamic.ObjectModel;

// ReSharper disable once CheckNamespace
namespace Automation.TestFramework;

/// <summary>
/// Provides access to the current expected result step.
/// </summary>
public abstract class ExpectedResultStep : AsyncLocalContainer<IDynamicStep>
{
    /// <summary>
    /// Gets the current value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the current value is not set by the test framework.</exception>
    public new static IExpectedResultStep Current
    {
        get
        {
            var currentStep = AsyncLocalContainer<IDynamicStep>.Current;
            return new Wrapper(currentStep);
        }
    }

    internal class Wrapper(IDynamicStep step) : IExpectedResultStep
    {
        public IExpectedResultDescriptor Descriptor => step.GetDescriptor<IExpectedResultDescriptor>();

        IStepDescriptor IDynamicStep.Descriptor => step.Descriptor;
        TStepDescriptor IDynamicStep.GetDescriptor<TStepDescriptor>() => step.GetDescriptor<TStepDescriptor>();
    }
}