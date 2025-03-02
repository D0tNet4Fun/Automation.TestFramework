using System;
using System.Threading;

namespace Automation.TestFramework.Dynamic;

public abstract class AsyncLocalContainer<T>
{
    private static AsyncLocal<T?> Local { get; } = new();

    /// <summary>
    /// Gets the current value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the current value is not set by the test framework.</exception>
    public static T Current => Local.Value ?? throw new InvalidOperationException($"Current {typeof(T).Name} is not set.");

    internal static void SetCurrent(T value) => Local.Value = value;
    internal static void ResetCurrent() => Local.Value = default;
}