using System;

namespace Automation.TestFramework.Dynamic;

/// <summary>
/// Thrown when a step fails because not all its sub-steps were successful.
/// </summary>
/// <param name="message">The error message.</param>
public class StepExecutionException(string message) : Exception(message)
{
    public override string StackTrace => string.Empty;
}