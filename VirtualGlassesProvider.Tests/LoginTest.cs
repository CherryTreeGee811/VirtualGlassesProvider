using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


namespace VirtualGlassesProvider.Tests
{
    [TestFixture, Order(2)]
    internal class LoginTest
    {
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
        public void ClientFailsToLogin()
        {
            _driver.Navigate().GoToUrl(AppServer.URL);
            _driver.Manage().Window.Size = new System.Drawing.Size(1936, 1056);
            _driver.FindElement(By.Id("login")).Click();
            _driver.FindElement(By.Id("Input_Email")).Click();
            _driver.FindElement(By.Id("Input_Email")).SendKeys(TestClient.Email);
            _driver.FindElement(By.Id("Input_Password")).Click();
            _driver.FindElement(By.Id("Input_Password")).SendKeys("Test1S");
            _driver.FindElement(By.Id("login-submit")).Click();
            Assert.That(_driver.FindElement(By.CssSelector(".text-danger li")).Text, Is.EqualTo("Invalid login attempt."));
        }


        [Test, Order(2)]
        public void ClientLogsInSuccessfully()
        {
            _driver.Navigate().GoToUrl(AppServer.URL);
            _driver.Manage().Window.Size = new System.Drawing.Size(1936, 1056);
            _driver.FindElement(By.Id("login")).Click();
            _driver.FindElement(By.Id("Input_Email")).Click();
            _driver.FindElement(By.Id("Input_Email")).SendKeys(TestClient.Email);
            _driver.FindElement(By.Id("Input_Password")).Click();
            _driver.FindElement(By.Id("Input_Password")).SendKeys(TestClient.Password);
            _driver.FindElement(By.Id("login-submit")).Click();
            Assert.That(_driver.FindElement(By.LinkText($"HELLO {TestClient.Email.ToUpper()}!")) != null);
        }
    }
}
