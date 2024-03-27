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


        #pragma warning disable NUnit1032
        private ChromeDriver _driver { get; set; }
        #pragma warning restore NUnit1032


        [SetUp]
        public void SetUp()
        {
            ChromeOptions options = new ChromeOptions { AcceptInsecureCertificates = true };
            options.AddArgument("--headless=new");
            _driver = new ChromeDriver(options);
        }


        [TearDown]
        protected void TearDown()
        {
            _driver.Quit();
        }


        [Test, Order(1)]
        public void ClientFailsToProvideAValidPasswordOnSignUp()
        {
            _driver.Navigate().GoToUrl(AppServer.URL);
            _driver.Manage().Window.Size = new System.Drawing.Size(1012, 991);
            _driver.FindElement(By.Id("register")).Click();
            _driver.FindElement(By.Id("Input_Email")).SendKeys(TestClient.Email);
            _driver.FindElement(By.Id("Input_Password")).SendKeys("password");
            _driver.FindElement(By.Id("Input_ConfirmPassword")).SendKeys("password");
            _driver.FindElement(By.Id("registerSubmit")).Click();
            Assert.That(_driver.FindElement(By.CssSelector(".text-danger li:nth-child(1)")).Text, Is.EqualTo("Passwords must have at least one non alphanumeric character."));
            Assert.That(_driver.FindElement(By.CssSelector(".text-danger li:nth-child(2)")).Text, Is.EqualTo("Passwords must have at least one digit (\'0\'-\'9\')."));
            Assert.That(_driver.FindElement(By.CssSelector("div.text-danger > ul:nth-child(1) > li:nth-child(3)")).Text, Is.EqualTo("Passwords must have at least one uppercase (\'A\'-\'Z\')."));
        }


        [Test, Order(2)]
        public void ClientSignsUpSuccessfully()
        {
            _driver.Navigate().GoToUrl(AppServer.URL);
            _driver.Manage().Window.Size = new System.Drawing.Size(1012, 991);
            _driver.FindElement(By.Id("register")).Click();
            _driver.FindElement(By.Id("Input_Email")).SendKeys(TestClient.Email);
            _driver.FindElement(By.Id("Input_Password")).SendKeys(TestClient.Password);
            _driver.FindElement(By.Id("Input_ConfirmPassword")).SendKeys(TestClient.Password);
            _driver.FindElement(By.Id("registerSubmit")).Click();
            Assert.That(_driver.FindElement(By.CssSelector("body > div > main > p")).Text, Is.EqualTo($"Please check your {TestClient.Email} email to confirm your account."));
        }
    }
}
