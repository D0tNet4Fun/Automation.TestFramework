using System;
using System.Threading.Tasks;

namespace Automation.TestFramework.Dynamic.ObjectModel;

/// <summary>
/// Allows adding sub steps from code, using a fluent API.
/// </summary>
public interface IStepDescriptor
{
    /// <summary>
    /// Executes code as a sub-step within the current step.
    /// </summary>
    /// <param name="type">The sub step type.</param>
    /// <param name="description">The description of the code to be executed.</param>
    /// <param name="code">The code.</param>
    /// <returns>The same step descriptor instance used to make this call.</returns>
    IStepDescriptor ExecuteSubStep(SubStepType type, string description, Action code);

    /// <summary>
    /// Executes async code as a sub-step within the current step.
    /// </summary>
    /// <param name="type">The sub step type.</param>
    /// <param name="description">The description of the code to be executed.</param>
    /// <param name="code">The code.</param>
    /// <returns>The same step descriptor instance used to make this call.</returns>
    IStepDescriptor ExecuteAsyncSubStep(SubStepType type, string description, Func<Task> code);
}