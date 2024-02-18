using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;


namespace VirtualGlassesProvider.Tests
{
    [TestFixture, Order(2)]
    internal sealed class LoginTest
    {
        #pragma warning disable NUnit1032
        private static IWebDriver s_driver { get; set; }
        #pragma warning restore NUnit1032

        [SetUp]
        public static void SetUp()
        {
            s_driver = new FirefoxDriver(new FirefoxOptions { AcceptInsecureCertificates = true });
        }


        [TearDown]
        protected static void TearDown()
        {
            s_driver.Quit();
        }


        [Test, Order(1)]
        public static void ClientFailsToLogin()
        {
            s_driver.Navigate().GoToUrl("https://localhost:7044/");
            s_driver.Manage().Window.Size = new System.Drawing.Size(1012, 691);
            s_driver.FindElement(By.Id("login")).Click();
            s_driver.FindElement(By.Id("Input_Email")).Click();
            s_driver.FindElement(By.Id("Input_Email")).SendKeys("TestClient@Sharklasers.com");
            s_driver.FindElement(By.Id("Input_Password")).Click();
            s_driver.FindElement(By.Id("Input_Password")).SendKeys("Test1S");
            s_driver.FindElement(By.Id("login-submit")).Click();
            Assert.That(s_driver.FindElement(By.CssSelector(".text-danger li")).Text, Is.EqualTo("Invalid login attempt."));
        }


        [Test, Order(2)]
        public static void ClientLogsInSuccessfully()
        {
            s_driver.Navigate().GoToUrl("https://localhost:7044/");
            s_driver.Manage().Window.Size = new System.Drawing.Size(1936, 1056);
            s_driver.FindElement(By.Id("login")).Click();
            s_driver.FindElement(By.Id("Input_Email")).Click();
            s_driver.FindElement(By.Id("Input_Email")).SendKeys("TestClient@Sharklasers.com");
            s_driver.FindElement(By.Id("Input_Password")).Click();
            s_driver.FindElement(By.Id("Input_Password")).SendKeys("Test1$");
            s_driver.FindElement(By.Id("login-submit")).Click();
            Assert.That(s_driver.FindElement(By.LinkText("HELLO TESTCLIENT@SHARKLASERS.COM!")) != null);
        }
    }
}
