using System;
using System.Threading.Tasks;

namespace Automation.TestFramework.Dynamic.ObjectModel;

/// <summary>
/// Allows adding sub steps from code, using a fluent API.
/// </summary>
public interface IStepDescriptor
{
    /// <summary>
    /// Adds code as a sub-step within the current step.
    /// </summary>
    /// <param name="description">The description of the code to be executed as a sub-step.</param>
    /// <param name="code">The code.</param>
    /// <returns>The same step descriptor instance used to make this call.</returns>
    IStepDescriptor AddSubStep(string description, Action code);

    /// <summary>
    /// Adds async code as a sub-step within the current step.
    /// </summary>
    /// <param name="description">The description of the code to be executed as a sub-step.</param>
    /// <param name="code">The code.</param>
    IStepDescriptor AddAsyncSubStep(string description, Func<Task> code);

    /// <summary>
    /// Executes code as a sub-step within the current step.
    /// </summary>
    /// <param name="description">The description of the code to be executed.</param>
    /// <param name="code">The code.</param>
    /// <returns>The same step descriptor instance used to make this call.</returns>
    IStepDescriptor ExecuteSubStep(string description, Action code);

    /// <summary>
    /// Executes async code as a sub-step within the current step.
    /// </summary>
    /// <param name="description">The description of the code to be executed.</param>
    /// <param name="code">The code.</param>
    /// <returns>The same step descriptor instance used to make this call.</returns>
    IStepDescriptor ExecuteAsyncSubStep(string description, Func<Task> code);

    /// <summary>
    /// Executes the sub-steps that were added since the previous execution.
    /// </summary>
    IStepDescriptor Execute();
}