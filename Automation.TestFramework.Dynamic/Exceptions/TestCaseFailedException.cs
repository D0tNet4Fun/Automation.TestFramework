using System;

// ReSharper disable once CheckNamespace
namespace Automation.TestFramework;

/// <summary>
/// Thrown when a test case fails because not all its steps were successful.
/// </summary>
/// <param name="message">The error message.</param>
public class TestCaseFailedException(string message) : Exception(message)
{
    public override string StackTrace => string.Empty;
}