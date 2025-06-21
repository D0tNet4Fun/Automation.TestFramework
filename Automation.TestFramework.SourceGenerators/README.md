# Automation.TestFramework.SourceGenerators
This is a source generator used to generate the `Summary` of a test case whose steps are named methods defined in a test class.

## Getting started
1. Given a test project configured to use `Automation.TestFramework.Dynamic` (aka `dynamic`), add the NuGet package `Automation.TestFramework.SourceGenerators`.
2. Create a test class in the same way as if using `Automation.TestFramework v1`.

For example:
```c#
using Automation.TestFramework;

[TestCase("TC001")] // this is cosmetic
public partial class TestCase1
{
    // define test data
    private const string WebsiteUrl = "https://my.site.com";
    private const string UserName = "user";
    private const string Password = "password";

    [Summary("Log in to website")]
    public partial void LoginToWebsite();

    [Precondition(1, "The user has an account on the website")]
    private void CreateUserAccount() {...}

    [Input(1, "The user has an account on the website")]
    private void OpenWebBrowser() {...} // use WebsiteUrl

    [Input(2, "Enter the user name")]
    private void EnterUserName() {...} // use Username

    [Input(3, "Enter the password")]
    private void EnterPassword() {...} // use Password

    [Input(4, "Click the Login button")]
    private void LogIn() {...}

    [ExpectedResult(4, "The user is logged in")]
    private void VerifyUserIsLoggedIn() {...}
}
```
This generates the `Summary` as:

```c#
public partial void LoginToWebsite()
{
    TestCase.Current.Descriptor
        .AddStep(StepType.Precondition, "The user has an account on the website", CreateUserAccount)
        .AddStep(StepType.Input, "The user has an account on the website", OpenWebBrowser)
        .AddStep(StepType.Input, "Enter the user name", EnterUserName)
        .AddStep(StepType.Input, "Enter the password", EnterPassword)
        .AddStep(StepType.Input, "Click the Login button", LogIn)
        .AddStep(StepType.ExpectedResult, "The user is logged in", VerifyUserIsLoggedIn)
        ;
}
```
The only difference from `v1` is that the `Summary` method is now partial, and so is the test class.

## Attributes
The source generator provides the same attributes as `v1`. These correspond to the 5 types of steps defined by `dynamic`:
- `[Setup]`
- `[Precondition]`
- `[Input]`
- `[ExpectedResult]`
- `[Cleanup]`

Each of these attributes defines:
- the order in which the step is to be executed, relative to other steps of the same type. If not specified, the default value is `1`.
- the step description, optional


## Features

### Async steps
The source generator detects if a step is sync or async.

For example, given this test class:
```c#

public partial class TestCase1
{
    [Summary("Log in to website")]
    public partial void LoginToWebSite();

    [Input(1, "The user logs in to the website" )]
    private void OpenWebBrowserAndLogin() {...}

    [ExpectedResult(1, "The user is logged in")]
    private Task VerifyUserIsLoggedIn() {...}
}
```
Then `Summary` will be generated as:
```c#
public partial void LoginToWebsite()
{
    TestCase.Current.Descriptor
        .AddStep(StepType.Input, "The user logs in to the website", OpenWebBrowserAndLogin)
        .AddAsyncStep(StepType.ExpectedResult, "The user is logged in", VerifyUserIsLoggedIn)
        ;
}
```

### Readability
All the test case attributes support specifying a description that shows in the test report. This is optional though.

If the description is missing, then the method name is used -- but not as is. It is 'humanized' using https://github.com/Humanizr/Humanizer.


### `[Setup]` and `[Cleanup]` can be defined in the class hierarchy
This works in the same way as if these two were class constructor and `Dispose`.

For example, given this test class hierarchy:
```c#
public class TestCaseBase
{
    [Setup(1, "Run this first")]
    public void RunThisFirst()
    {
    }
    
    [Cleanup(1, "Run this last")]
    public void RunThisLast()
    {
    }
}

public partial class TestCase1 : TestCaseBase
{
    [Summary("Log in to website")]
    public partial void LoginToWebsite();

    [Input(1, "The user logs in to the website" )]
    private void OpenWebBrowserAndLogin() {...}

    [ExpectedResult(1, "The user is logged in")]
    private Task VerifyUserIsLoggedIn() {...}
    
    [Cleanup(1, "Close browser")]
    private void CloseBrowser() { ... }
}
```
Then the `Summary` method will be generated to include steps from the base class:
```c#
public partial void LoginToWebsite()
{
    TestCase.Current.Descriptor
        .AddStep(StepType.Setup, "Run this first", RunThisFirst)
        .AddStep(StepType.Input, "The user logs in to the website", OpenWebBrowserAndLogin)
        .AddAsyncStep(StepType.ExpectedResult, "The user is logged in", VerifyUserIsLoggedIn)
        .AddStep(StepType.Cleanup, "Close browser", CloseBrowser)
        .AddStep(StepType.Cleanup, "Run this last", RunThisLast)
        ;
}
```
The discovery of `[Setup]` and `[Cleanup]` walks the entire test class hierarchy. 
