using System;

namespace Automation.TestFramework.Dynamic;

/// <summary>
/// Encapsulates events that can be raised by the framework.
/// </summary>
public class EventSource
{
    /// <summary>
    /// Gets the single instance of this class.
    /// </summary>
    public static EventSource Instance { get; } = new();

    /// <summary>
    /// Raised when an exception occurs while running a step.
    /// </summary>
    public event EventHandler<Exception>? StepError;

    internal void OnStepError(object? testClassInstance, Exception error)
    {
        StepError?.Invoke(testClassInstance, error);
    }
}