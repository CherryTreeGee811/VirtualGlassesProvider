using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


namespace VirtualGlassesProvider.Tests
{
    [TestFixture, Order(1)]
    internal class SignupTest
    {
        // FYI the suite is meant to be run cohesively to ensure proper clearing of resources
        // and that the application is harmonious. To run the suite cohesively please select run all tests
        // in the tests exployer.
        // Thank you

        private ChromeDriver Driver { get; set; }


        [SetUp]
        public void SetUp()
        {
            var options = new ChromeOptions { AcceptInsecureCertificates = true };
            options.AddArgument("--headless=new");
            Driver = new ChromeDriver(options);
        }


        [TearDown]
        protected void TearDown()
        {
            Driver.Quit();
            Driver.Dispose();
        }


        [Test, Order(1)]
        public void ClientFailsToProvideAValidPasswordOnSignUp()
        {
            Driver.Navigate().GoToUrl(AppServer.URL);
            TestUtils.ClickElementSafely("register", Driver);
            Driver.FindElement(By.Id("Input_Email")).SendKeys(TestClient.Email);
            Driver.FindElement(By.Id("Input_Password")).SendKeys("password");
            Driver.FindElement(By.Id("Input_ConfirmPassword")).SendKeys("password");
            TestUtils.ClickElementSafely("registerSubmit", Driver);
            Assert.Multiple(() =>
            {
                Assert.That(Driver.FindElement(By.CssSelector("#registerRequestErrorMessage li:nth-child(1)")).Text, Is.EqualTo("Passwords must have at least one non alphanumeric character."));
                Assert.That(Driver.FindElement(By.CssSelector("#registerRequestErrorMessage li:nth-child(2)")).Text, Is.EqualTo("Passwords must have at least one digit (\'0\'-\'9\')."));
                Assert.That(Driver.FindElement(By.CssSelector("#registerRequestErrorMessage li:nth-child(3)")).Text, Is.EqualTo("Passwords must have at least one uppercase (\'A\'-\'Z\')."));
            });
        }


        [Test, Order(2)]
        public void ClientSignsUpSuccessfully()
        {
            Driver.Navigate().GoToUrl(AppServer.URL);
            Driver.Manage().Window.Size = new System.Drawing.Size(Display.DesktopWidth, Display.DesktopHeight);
            TestUtils.ClickElementSafely("register", Driver);
            Driver.FindElement(By.Id("Input_Email")).SendKeys(TestClient.Email);
            Driver.FindElement(By.Id("Input_Password")).SendKeys(TestClient.Password);
            Driver.FindElement(By.Id("Input_ConfirmPassword")).SendKeys(TestClient.Password);
            TestUtils.ClickElementSafely("registerSubmit", Driver);
            TestUtils.WaitForElementToBeVisible(By.Id("registerConfirmationCheckEmailMessage"), Driver);
            Assert.That(Driver.FindElement(By.Id("registerConfirmationCheckEmailMessage")).Text, Is.EqualTo($"Please check your {TestClient.Email} email to confirm your account."));
        }
    }
}
