# Automation.TestFramework
This is a test framework based on [xUnit.net](http://xunit.github.io). The framework is designed for automated tests that are written based on test cases. 

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
Note: this changes a fundamental concept of xUnit, where a test method is viewed as a test case and the test class is viewed as a collection of related test cases. For us, the test case is the test class.

### Running the test case
When the test runner discovers the test case, the only test method visible is the one marked as *Summary*. This is rendered using the description, i.e. _Login to web site_.

When the Summary method is executed, all the other test methods are discovered and they show as tests linked to the Summary test method. Their names are rendered using their descriptions.

This way, the test report matches the test case definition as closely as possible.

## How to use in VS 2017
Create a new xUnit test project for .NET Core and then add a reference to Automation.TestFramework. The package is also available on NuGet.org.

Add the following code:
```C#
[assembly: TestFramework("Automation.TestFramework.Framework", "Automation.TestFramework")]
```

**Note**: make sure the project dependencies include the ones mentioned on http://xunit.github.io/docs/getting-started-dotnet-core.html#run-tests-visualstudio.


## Supported frameworks
The test framework supports .NET 4.5.2 or later and .NET Core 2.1.

## Test runner compatibility
The test framework works with all test runners supported by xUnit.

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
The test framework preserves all of xUnit's features related to sharing context between tests (see https://xunit.github.io/docs/shared-context.html). Therefore let's leave fixtures out of this.

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

## Parallelism
The test framework has the same behavior as xUnit, with one exception: _test cases that are in the same collection run in parallel_, unless specified otherwise. To disable parallelization for a test collection:

```C#
[CollectionDefinition("Serial", DisableParallelization = true)]
public class CollectionDefinition : ICollectionFixture<CollectionFixture> { }

[Collection("Serial")]
[TestCase] public class TestCase1 {}

[Collection("Serial")]
[TestCase] public class TestCase2 {}
```

Note: it is a common scenario for more test cases to share a context, which is implemented as xUnit collection fixture. This implies the use of a collection definition and this way the test cases get to be part of the same collection, although they are in different classes.

## Assertions in expected results
Sometimes expected results may consist of more than one assertions whose outcomes determine if the test step passes or fails. I.e. consider a basic test case such as:
```
Precondition: User logs in
Input: User goes to the Profile page
Expected result: The user display name and email are correct
```

The expected result verifies 2 things: the user display name and the email address. They both need to be correct for the test to pass. They both need to be visible in the test report, in case one of them fails. The failure may be considered critical, or not.

### Assertions
For the above test case, assume that when either of the user display name / email is incorrect then the other one does not need to be verified - the test fails anyways. This can be written as:
```C#
[ExpectedResult]
private void ExpectedResult()
{
	TestStep.Current.ExpectedResult
		.Assert("Expect the user display name is correct", () => Assert.[...])
		.Assert("Expect the email is correct", () => Assert.[...]);
}
```
This code produces two tests for the test step. 

When they both pass then the test report contains:
```
[3/3] Expected result: 1. The user display name and email are correct - passed
[3/3] [Expected result] 1.1. Expect the user display name is correct - passed
[3/3] [Expected result] 1.2. Expect the email is correct - passed
```

When an assertion fails then the failure is shown in the test report, the next assertions are not executed at all, and the test step fails with a specific error. I.e. when the user display name is not correct:
```
[3/3] Expected result: 1. The user display name and email are correct - failed: One or more of the expected results did not match. 1 assertion(s) were skipped.
[3/3] [Expected result] 1.1. Expect the user display name is correct - failed
```

### Verifications
For the above test case, assume that when one of the user display name / email is incorrect then the other needs to be checked too before the test fails. This can be written as:
```C#
[ExpectedResult]
private void ExpectedResult()
{
	TestStep.Current.ExpectedResult
		.Verify("Expect the user display name is correct", () => Assert.[...])
		.Verify("Expect the email is correct", () => Assert.[...]);
}
```
When a verification fails then the failure is shown in the test report and the next assertion/verification is executed. I.e. when the user display name is not correct but the email is, then:
```
[3/3] Expected result: 1. The user display name and email are correct - failed: One or more of the expected results did not match
[3/3] [Expected result] 1.1. Expect the user display name is correct - failed
[3/3] [Expected result] 1.2. Expect the email is correct - passed
```

### Execution
Assertions and verifications are executed as soon as possible, on the same thread as the rest of the method. I.e. given the above test method, the order of execution is:
1. Call 1st Verify
2. Call 1st delegate
3. Call 2nd Verify
4. Call 2nd delegate

For asserts it is slighly different. As soon as a delegate throws, the next delegates are not called anymore but their assertions continue to be called in order to track how many assertions were skipped.

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
Assembly fixtures are created before any of the tests in the assembly are run, similar to xUnit's collection fixtures. 
Use attributes to specify them:
```C#
[assembly: AssemblyFixture(typeof(AssemblyFixtureClass))]
```

Note: this is copied from xUnit's Sample project.