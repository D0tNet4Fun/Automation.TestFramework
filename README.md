# Automation.TestFramework
This is a test framework designed for automated tests based on test cases. 

The framework allows the user to define a test case to a test class. The test case preconditions and steps can be mapped as methods. The framework ensures they are executed in the correct order.

## Test case definition
Consider a test case based on the template at http://www.softwaretestinghelp.com/test-case-template-examples:

Test case ID: TC001

Test case summary: Login to web site

Precondition: The user has an account on the web site

Step | Test step | Test data | Expected result | Actual result | Status
--- | --- | --- | --- | --- | --- 
1 | Open a web browser and navigate to the web site | http://my.site.com | | | |
2 | Enter the user name | user | | | |
3 | Enter password | password | | | |
4 | Click the Login button | | The user is logged in | |

We can define this test case as a test class:
```c#
[TestCase("TC001")]
public class TestCase1
{
	// define test data
	private const string WebSiteUrl = "http://my.site.com";
	private const string UserName = "user";
	private const string Password = "password";

	[Summary("Login to web site")]
	public void LoginToWebSite() { } // leave this empty
	
	[Precondition("The user has an account on the web site")]
	private void CreateUserAccount() {...}
	
	[Input(1, "Open a web browser and navigate to the web site")]
	private void OpenWebBrowser() {...} // use WebSiteUrl
	
	[Input(2, "Enter the user name")]
	private void EnterUserName() {...} // use Username
	
	[Input(3, "Enter password")]
	private void EnterPassword() {...} // use Password
	
	[Input(4, "Click the Login button")]
	private void LogIn() {...}
	
	[ExpectedResult(4, "The user is logged in")]
	private void VerifyUserIsLoggedIn() {...}
}
```
Note: this changes a fundamental concept of Xunit, where a test method is viewed as a test case and the test class is viewed as a collection of related test cases. For us, the test case is the test class.

### Running the test case
When the test runner discovers the test case, the only test method visible is the one marked as *Summary*. This is rendered using the description, i.e. _Login to web site_.

When the Summary method is executed, all the other test methods are discovered and they show as tests linked to the Summary test method. Their names are rendered using their descriptions.

This way, the test report matches the test case definition as closely as possible.

## How to use in VS 2017
Create a new Xunit test project for .NET Core and then add a reference to Automation.TestFramework. The package is also available on NuGet.org.

Add the following code:
```C#
[assembly: TestFramework("Automation.TestFramework.Framework", "Automation.TestFramework")]
```

**Note**: make sure the project dependencies include the ones mentioned on http://xunit.github.io/docs/getting-started-dotnet-core.html#run-tests-visualstudio.


## Supported frameworks
The test framework supports .NET Core 2.0.

## Test runner compatibility
The test framework is based on Xunit. It works with all test runners supported by Xunit.

There is a _known issue_ with the Resharper test runner: the tests do not show under the Summary method. This is being researched.

## Test case attributes
The test framework uses attributes to identify test cases and their components:

**TestCase**

Identifies a test class as a test case. The classes not marked as such are ignored by the test runner.

**Summary**

Used as the 'entry point' of the test case, that can be discovered by the test runner. Each test case class must have exactly one test method marked as Summary.

**Precondition**

Identifies a precondition of the test case. There can be more than one.

**Input**

Identifies the input of a test case step. There can be more than one.

**ExpectedResult**

Identifies the expected result of a test case step. Each expected result is linked to an input, but an input does not have to have an expected result.

### Setup and Cleanup
**Setup**

Used to identify methods that run before all of the other methods of the test case. There can be more than one.

**Cleanup**

Used to identify methods that run after all of the other methods of the test case have run, regardless of their outcome.

These two are optional, as this behavior is already implemented in Xunit using class constructor and Dispose. These should be used only if we need the test methods to show in the test report. 

I.e. consider a scenario in which the setup can take a long time to execute. If `[Setup]` is used then this will show as a test on the test report.

## Test case execution
The test framework preserves all of Xunit's features related to sharing context between tests (see https://xunit.github.io/docs/shared-context.html). Therefore let's leave fixtures out of this.

Here's what happens when the Summary method is executed:
1. The test case instance is created. _This will be shared between all tests_.
2. The tests are discovered.
3. The tests are ordered by type: Setup < Precondition < Test steps (Input and ExpectedResult) < Cleanup.
4. The tests grouped by type are ordered based on their definition. I.e. Precondition(1) < Precondition(2).
5. The test steps are ordered by test step. I.e. Input(2) < ExpectedResult(2) < Input(3).
6. The tests are run in order. If one fails then the next tests are *skipped*, except the Cleanup tests.
7. The Summary is executed. If one of the tests failed then the Summary fails with error: _The test case steps were not completed successfully_
7. The test case instance is disposed.

Note: The tests may execute on different threads, although not in parallel.

## Other features

### Readability
All of the test case attributes support specifying a description that shows in the test report. This is optional though.

If the description is missing then the method name is used - but not as is. It is 'humanized' using https://github.com/Humanizr/Humanizer.

```C#
[Summary]
public void LoginToWebSite() {...}
```
The name of this test as shown in the test report will be "Login to web site".

Also, all of the test case attributes support specifying the order in which the test is run. If not specified then this defaults to `1`. 
This can work for simple test cases such as:
```C#
[TestCase] public class TestCase 
{
	[Summary] public void Summary() { }
	[Precondition] private void Precondition() { }
	[Input] private void Input() { } 
	[ExpectedResult] private void ExpectedResult() { }
}
```

### Notifications
Notifications allow the user to access the exception that caused the test to fail, at that time (not later). To enable notifications:
1. Create a class that implements interface `ITestNotification`:
```C#
public class MyTestNotification : ITestNotification
{
	public MyTestNotification(object testClassInstance) {...} // constructor must have this signature

	public void OnError(Exception error) {...} // the exception
}
```
2. Use attributes to configure this using attributes:
```C#
[assembly: TestNotification(typeof(MyTestNotification))] // to enable for all test cases in the assembly
// OR
[TestCase("TC001")]
[TestNotification(typeof(MyTestNotification)] // to enable for this specific test case
public class TestCase1 {...}
```
When the exception occurs, the test frameowrk will create an instance of `MyTestNotification` and pass it the test case instance. This context should be enough for the user to observe the exception. Note that the exception _cannot be handled_ and it will be rethrown regardless what the notification does.

### Assembly fixtures
Assembly fixtures are created before any of the tests in the assembly are run, similar to Xunit's collection fixtures. 
Use attributes to specify them:
```C#
[assembly: AssemblyFixture(typeof(AssemblyFixtureClass))]
```

Note: this is copied from Xunit's Sample project.