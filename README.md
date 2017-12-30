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
[TestCase("TC0001")]
public class TestCase1
{
	// define test data
	private const string WebSiteUrl = "http://my.site.com";
	private const string UserName = "Username";
	private const string Password = "Password";

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

## Test case execution
When the test runner discovers the test case, the only test method visible is the one marked as *Summary*. This is rendered using the description, i.e. _Login to web site_.

When the Summary method is executed, all the other test methods are discovered and they show as tests linked to the Summary test method. Their names are rendered using their descriptions.

This way, the test report matches the test case definition as closely as possible.