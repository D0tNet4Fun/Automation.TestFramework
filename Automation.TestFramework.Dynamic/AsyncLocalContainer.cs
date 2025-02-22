using System;
using System.Threading;

namespace Automation.TestFramework.Dynamic;

public abstract class AsyncLocalContainer<T>
{
    private static AsyncLocal<T?> Local { get; } = new();

    public static T Current => Local.Value ?? throw new InvalidOperationException($"Current {typeof(T).Name} is not set.");

    public static TResult GetCurrentAs<TResult>()
        where TResult : T
    {
        if (Local.Value is null)
            throw new InvalidOperationException($"Current {typeof(T).Name} is not set.");
        if (Local.Value is not TResult result)
            throw new InvalidOperationException($"Current {typeof(T).Name} mismatch. Expected: {typeof(TResult).Name}, actual: {Local.Value!.GetType().Name}.");

        return result;
    }

    internal static void SetCurrent(T value) => Local.Value = value;
    internal static void ResetCurrent() => Local.Value = default;
}