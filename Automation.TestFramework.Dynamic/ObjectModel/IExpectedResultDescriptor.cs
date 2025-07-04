using System;
using System.Threading.Tasks;

namespace Automation.TestFramework.Dynamic.ObjectModel;

/// <summary>
/// Allows running sub-steps from code within the current <see cref="StepType.ExpectedResult"/> step, using a fluent API.
/// </summary>
public interface IExpectedResultDescriptor : IStepDescriptor
{
    /// <summary>
    /// Executes code as a <see cref="SubStepType.Assertion"/> within the current step.
    /// </summary>
    /// <param name="description">The description of the code to be executed.</param>
    /// <param name="code">The code.</param>
    /// <returns>The same step descriptor instance used to make this call.</returns>
    public IExpectedResultDescriptor Assert(string description, Action code);
    
    /// <summary>
    /// Executes async code as a <see cref="SubStepType.Assertion"/> within the current step.
    /// </summary>
    /// <param name="description">The description of the code to be executed.</param>
    /// <param name="code">The code.</param>
    /// <returns>The same step descriptor instance used to make this call.</returns>
    public IExpectedResultDescriptor AssertAsync(string description, Func<Task> code);

    /// <summary>
    /// Executes code as a <see cref="SubStepType.Verification"/> within the current step.
    /// </summary>
    /// <param name="description">The description of the code to be executed.</param>
    /// <param name="code">The code.</param>
    /// <returns>The same step descriptor instance used to make this call.</returns>
    public IExpectedResultDescriptor Verify(string description, Action code);
    
    /// <summary>
    /// Executes async code as a <see cref="SubStepType.Verification"/> within the current step.
    /// </summary>
    /// <param name="description">The description of the code to be executed.</param>
    /// <param name="code">The code.</param>
    /// <returns>The same step descriptor instance used to make this call.</returns>
    public IExpectedResultDescriptor VerifyAsync(string description, Func<Task> code);
}