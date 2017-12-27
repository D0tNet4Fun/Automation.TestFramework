namespace Automation.TestFramework.Tests
{
    [TestCase("TC0002")]
    public class SecondTestCase
    {
        [Summary("Login to web site")]
        public void LoginToWebsite()
        {

        }

        [Precondition(1, "The web site must be deployed on the web server")]
        public void DeployWebSite()
        {

        }

        [Precondition(2, "The web site must be accessible via HTTPS")]
        public void EnableHttps()
        {

        }

        [Input(1, "Access the web site")]
        public void AccessWebSite()
        {

        }

        [Input(2, "Enter user name and password")]
        public void EnterUserNameAndPassword()
        {

        }

        [Input(3, "Click the login button")]
        public void Login()
        {

        }

        [ExpectedResult(3, "The user should be logged in")]
        public void VerifyUserIsLoggedIn()
        {
        }
    }
}