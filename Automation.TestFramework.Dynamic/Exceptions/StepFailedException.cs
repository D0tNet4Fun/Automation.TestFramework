using System;

// ReSharper disable once CheckNamespace
namespace Automation.TestFramework;

/// <summary>
/// Thrown when a step fails because not all its sub-steps were successful.
/// </summary>
/// <param name="message">The error message.</param>
public class StepFailedException(string message) : Exception(message)
{
    public override string StackTrace => string.Empty;
}