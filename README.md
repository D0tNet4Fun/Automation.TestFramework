# Automation.TestFramework.Dynamic
This is a test framework based on [xUnit.net v3](https://xunit.net/docs/getting-started/v3/cmdline) that allows defining a test case as a sequence of steps and sub-steps. These steps and sub-steps are based on code defined either as named methods or closures.

The framework is designed for automated tests that are written based on test cases. 

## Test case definition
Consider a test case based on the template at http://www.softwaretestinghelp.com/test-case-template-examples:

```
Test case ID: TC001

Test case summary: Log in to website

Precondition: The user has an account on the website

| Step | Test step                                      | Test data           | Expected result       | Actual result | Status |
|------|------------------------------------------------|---------------------|-----------------------|---------------|--------|
| 1    | Open a web browser and navigate to the website | https://my.site.com |                       |               |        |
| 2    | Enter the user name                            | user                |                       |               |        |
| 3    | Enter password                                 | password            |                       |               |        |
| 4    | Click the Login button                         |                     | The user is logged in |               |        |
```
This test case can be defined as a test class:
```c#
[TestCase("TC001")] // this is cosmetic
public class TestCase1
{
    // define test data
    private const string WebsiteUrl = "https://my.site.com";
    private const string UserName = "user";
    private const string Password = "password";

    [Summary("Log in to website")]
    public void LoginToWebSite()
    {
        // describe the test case steps
        TestCase.Current.Descriptor
            .AddStep(StepType.Precondition, "The user has an account on the website", CreateUserAccount)
            .AddStep(StepType.Input, "The user has an account on the website", OpenWebBrowser)
            .AddStep(StepType.Input, "Enter the user name", EnterUserName)
            .AddStep(StepType.Input, "Enter the password", EnterPassword)
            .AddStep(StepType.Input, "Click the Login button", LogIn)
            .AddStep(StepType.ExpectedResult, "The user is logged in", VerifyUserIsLoggedIn)
            ;
    }

    private void CreateUserAccount() {...}

    private void OpenWebBrowser() {...} // use WebsiteUrl

    private void EnterUserName() {...} // use Username

    private void EnterPassword() {...} // use Password

    private void LogIn() {...}

    private void VerifyUserIsLoggedIn() {...}
}
```
Note: this changes a fundamental concept of xUnit, where a test method is viewed as a test case and the test class is viewed as a collection of related test cases. For us, the test case is the test class.

## Getting started
Create a new xUnit test project as explained in https://xunit.net/docs/getting-started/v3/cmdline#create-the-unit-test-project.

Then add the NuGet package `Automation.TestFramework.Dynamic`.

### Supported frameworks
The test framework has the same target framework as `xUnit v3`:
- .NET Standard 2.0
- .NET Framework 4.7.2
- .NET 6.0

### Discovering the test case
The test method marked as `[Summary]` is automatically discovered by the xUnit test runner, as it was a `[Fact]`.

The actual test case steps are **not** discovered until the test is executed, which is why they are called dynamic tests. 

### Executing the test case
Run the `Summary` test to start discovering the actual test case steps based on closures. 
These closures are executed by the test framework in the order in which they were added when describing the test case.
Each closure is wrapped inside a test result linked to the `Summary` test. The display names of these tests are given by the description used when adding the steps.

This way, the test report matches the test case definition as closely as possible.

## Test case definition
### Attributes
The test framework uses attributes to identify test cases and their components:

- `[TestCase]`: identifies a test class as a test case. This is purely cosmetic and can be omitted.
- `[Summary]`: used as the 'entry point' of the test case that can be discovered by the test runner. 

Each test case class should have a single test method marked as Summary.

### Steps
The test case is defined as a sequence of steps. A step is defined by:
- the step type
- a description (used as display name)
- the code that implements the step

The test framework defines 5 types of steps:
- Setup
- Precondition
- Input
- Expected result
- Cleanup

The code that implements the step can be anything. For example:
- a method from the test class
- a static method from another class
- a closure

#### Execution
The steps are executed in the order they are added to the current test case.

If a step fails before other steps are executed, then the other steps are executed as skipped tests, except for the `Cleanup` steps. These are always executed.

#### Adding steps
Steps are added to the current test case inside the `Summary` method.
```c#
[Summary]
public void Summary
{
    TestCase.Current.Descriptor
        .AddStep(StepType.Input, "This is the input", Input)
}

private void Input() { ... }
```
If the code that implements the step returns a `Task` or `ValueTask`, then the step is considered async.
Async steps are added using `.AddAsyncStep()`:

```c#
[Summary]
public void Summary
{
    TestCase.Current.Descriptor
        .AddStep(StepType.Input, "This is the input", Input)
        .AddAsyncStep(StepType.ExpectedResult, "This is the expected result", ExpectedResult);
}

private void Input() { ... }
private Task ExpectedResult() { ... }
```

A test case can have both sync and async steps.

### Sub-steps
Each step can have sub-steps. These are dynamic tests that run during the current step execution.

```c#
private void Input()
{
    int value = 1;

    Step.Current.Descriptor
        .AddSubStep("Phase 1", () => { value = 2; })
        .AddAsyncSubStep("Phase 2 (async)", async() => { await ... })
        .Execute();

    Assert.Equal(2, value);
    value = 3;

    Step.Current.Descriptor
        .ExecuteubStep("Phase 3", () => { Assert.Equal(3, value); })
        .ExecuteAsyncSubStep("Phase 4 (async)", async() =>  { await ... });
}
```
This is similar to the way steps are added to the current test case. 
There is a difference though: the execution of the sub-steps can be mixed with the execution of the current step, by calling `Execute` methods. This allows executing a sequence of sub-steps or even one sub-step at a time.

If no `Execute` method is called after adding all sub-steps, then the test framework executes all of them in order. 

If a sub-step fails before other sub-steps are executed, then the other sub-steps are executed as skipped tests (unless the failed sub-step is a verification, see below).

### Assertions and verifications
These are special sub-steps used in `ExpectedResult` steps when they consist of multiple assertions whose outcomes determine if the test step passes or fails.

For example, consider a basic test case such as:
```
Precondition: User logs in
Input: User goes to the Profile page
Expected result: The user display name and email are correct
```

The expected result verifies 2 things: the user display name and the email address. They both need to be correct for the test to pass. They both need to be visible in the test report, in case one of them fails. The failure may be considered critical, or not.

#### Assertions
For the above test case, assume that when either of the user display name / email is incorrect then the other one does not need to be verified - the test fails anyway. This can be written as:
```C#
private void ExpectedResult()
{
    Step.Current.GetDescriptor<IExpectedResultDescriptor>()
        .Assert("Expect the user display name is correct", () => Assert.[...])
        .AssertAsync("Expect the email is correct", async () => 
        { 
            await ...  
            Assert.[...]
        });
}
```
This code produces two tests for the test step. 

When they both pass then the test report contains:
```
[3/3] Expected result] 1. The user display name and email are correct - passed
[3/3] [Expected result] 1.1. Expect the user display name is correct - passed
[3/3] [Expected result] 1.2. Expect the email is correct - passed
```

When an assertion fails then the failure is shown in the test report, the next assertions are not executed at all, and the test step fails with a specific error. I.e. when the user display name is not correct:
```
[3/3] [Expected result] 1. The user display name and email are correct - failed: One or more of the expected results did not match. 1 assertion(s) were skipped.
[3/3] [Expected result] 1.1. Expect the user display name is correct - failed
```

#### Verifications
For the above test case, assume that when one of the user display name / email is incorrect then the other needs to be checked too before the test fails. This can be written as:
```C#
[ExpectedResult]
private void ExpectedResult()
{
    Step.Current.GetDescriptor<IExpectedResultDescriptor>()
       .Verify("Expect the user display name is correct", () => Assert.[...])
       .VerifyAsync("Expect the email is correct", async () => 
        { 
            await ...  
            Assert.[...]
        });
}
```
When a verification fails, then the failure is shown in the test report and the next assertion/verification is executed. I.e. when the user display name is not correct but the email is, then:
```
[3/3] [Expected result] 1. The user display name and email are correct - failed: One or more of the expected results did not match
[3/3] [Expected result] 1.1. Expect the user display name is correct - failed
[3/3] [Expected result] 1.2. Expect the email is correct - passed
```

## Other features

### Readability
The `[Summary]` attribute supports specifying a description that is used as the test display name.

If this description is missing then the method name is used - but not as is. It is 'humanized' using https://github.com/Humanizr/Humanizer.

```C#
[Summary]
public void LoginToWebsite() {...}
```
The name of this test as shown in the test report will be "Log in to website".

### Events
The test framework raises events that the user code can handle using `EventSource.Instance`.

For example, consider this scoped class:
```c#
using Automation.TestFramework.Dynamic; // needed for EventSource

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
        // semder is the instance used to invoke the step method or closure, or null if static
        // e is the exception
    }
}
```
This class can be used with any of the xUnit fixtures, such as:
- `AssemblyFixture(Type)`
- `ICollectionFixture<>`
- `IClassFixture<>
`