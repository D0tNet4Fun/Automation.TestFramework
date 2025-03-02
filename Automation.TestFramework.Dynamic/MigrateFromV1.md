# Migrate from `Automation.TestFramework` 

`Automation.TestFramework` (aka `v1` going forward) modified xUnit in a more complex way than 
`Automation.TestFramework.Dynamic` (aka `dynamic`) needs to.

## Key differences

### Not a custom `TestFramework`
`dynamic` is not a custom xUnit `TestFramework` that overrides xUnit's executor / assembly runner / collection runner. Therefore, this code is no longer needed and must be removed:
```c#
[assembly: TestFramework("Automation.TestFramework.Framework", "Automation.TestFramework")]
```

### Steps are not attributes
Unlike `v1` that defined attributes such as `[InputAttribute]`, `dynamic` provides step types such as `StepType.Input` that are used inside the `Summary` test method. 

While this looks like a **breaking change**, do not worry: the same functionality can be achieved using `Automation.TestFramework.SourceGenerators`. This being said, `dynamic` does not provide these custom attributes anymore. 

### Parallelism
`dynamic` uses the same parallelism as xUnit, unlike `v1`. In `v1`, the tests that were part of the same collection used to run in parallel. This is no longer the case with `dynamic`. 
If the goal is to use a collection as a shared context provider, then this should be used with an *assembly fixture*.

For example, instead of:
```c#
[CollectionDefinition("MyCollection")]
public class Collection : ICollectionFixture<Context>

[Collection("MyCollection")]
public class TestCaseBase) { ... }

public class TestCase1 : TestCaseBase { ... }
public class TestCase2 : TestCaseBase { ... }
```
do:
```c#
[assembly: AssemblyFixture(typeof(Context))]

class Context : IDisposable
{
    public static Context Current { get; private set; }

    public Context() // called by xUnit once, before all tests start
    {
        Instance = this;
    }
    
    public void Dispose() // also called by xUnit once, after all tests complete
    {
    }
}

public class TestCaseBase { ... } // no collection on base class!

public class TestCase1 : TestCaseBase { ... }
public class TestCase2 : TestCaseBase { ... }
```
This ensures `TestCase1` and `TestCase2` run in parallel when using xUnit's default behavior of having a test collection per class.

Note: this can be different from `v1` if several collections are used, as this will keep the shared context alive beyond the original collection scope. 

### `EventSource` instead of notifications
`v1`'s `TestNtification` attribute and `***TestNotification` classes are replaced by `EventSource`, whose single instance can be used to handle events raised by `dynamic`.

For example, instead of:
```c#
[assembly: TestNotification(typeof(MyNotification))]
```
do:
```c#
[assembly: AssemblyFixture(typeof(EventHandlers))]

public class EventHandlers : IDisposable
{
    public EventHandlers()
    {
        EventSource.Instance.StepError += OnStepError;
    }

    public void Dispose()
    {
        EventSource.Instance.StepError -= OnStepError;
    }

    private void OnStepError(object sender, Exception e)
    {
        var notification = new MyNotification(sender);
        notification.OnError(e);
    }
}
```

### `Step` instead of `TestStep`
`v1`'s `TestStep.Current` is replaced by `Step.Current` to support generic sub-steps beyond assertions and verifications.

For example, instead of:
```c#
private void ExpectedResult()
{
    TestStep.Current.ExpectedResult
        .Assert(...)
}
```
do:
```c#
private void ExpectedResult()
{
    Step.Current.GetDescriptor<IExpectedResultDescriptor>()
        .Assert(...)
}
```
Note: this is optional, as `dynamic` provides backward compatibility by defining a `TestStep` class that is marked as obsolete.

### `AssemblyFixture` is now defined in xUnit v3
```c#
using Xunit;
[assembly: AssemblyFixture(...)]
```
