using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;


namespace VirtualGlassesProvider.Tests
{
    [TestFixture, Order(1)]
    internal sealed class SignupTest
    {
        // FYI the suite is meant to be run cohesively to ensure proper clearing of resources
        // and that the application is harmonious. To run the suite cohesively please select run all tests
        // in the tests exployer.
        // Thank you


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
        public static void ClientFailsToProvideAValidPasswordOnSignUp()
        {
            s_driver.Navigate().GoToUrl("https://localhost:7044/");
            s_driver.Manage().Window.Size = new System.Drawing.Size(1012, 991);
            s_driver.FindElement(By.Id("register")).Click();
            s_driver.FindElement(By.Id("Input_Email")).Click();
            s_driver.FindElement(By.Id("Input_Email")).SendKeys("TestClient@Sharklasers.com");
            s_driver.FindElement(By.Id("Input_Password")).Click();
            s_driver.FindElement(By.Id("Input_Password")).SendKeys("password");
            s_driver.FindElement(By.Id("Input_ConfirmPassword")).Click();
            s_driver.FindElement(By.Id("Input_ConfirmPassword")).SendKeys("password");
            s_driver.FindElement(By.Id("registerSubmit")).Click();
            Assert.That(s_driver.FindElement(By.CssSelector(".text-danger li:nth-child(1)")).Text, Is.EqualTo("Passwords must have at least one non alphanumeric character."));
            Assert.That(s_driver.FindElement(By.CssSelector(".text-danger li:nth-child(2)")).Text, Is.EqualTo("Passwords must have at least one digit (\'0\'-\'9\')."));
            Assert.That(s_driver.FindElement(By.CssSelector("li:nth-child(3)")).Text, Is.EqualTo("Passwords must have at least one uppercase (\'A\'-\'Z\')."));
        }


        [Test, Order(2)]
        public static void ClientSignsUpSuccessfully()
        {
            s_driver.Navigate().GoToUrl("https://localhost:7044/");
            s_driver.Manage().Window.Size = new System.Drawing.Size(1012, 991);
            s_driver.FindElement(By.Id("register")).Click();
            s_driver.FindElement(By.Id("Input_Email")).Click();
            s_driver.FindElement(By.Id("Input_Email")).SendKeys("TestClient@Sharklasers.com");
            s_driver.FindElement(By.Id("Input_Password")).Click();
            s_driver.FindElement(By.Id("Input_Password")).SendKeys("Test1$");
            s_driver.FindElement(By.Id("Input_ConfirmPassword")).Click();
            s_driver.FindElement(By.Id("Input_ConfirmPassword")).SendKeys("Test1$");
            s_driver.FindElement(By.Id("registerSubmit")).Click();
            Assert.That(s_driver.FindElement(By.CssSelector("p")).Text, Is.EqualTo("Please check your TestClient@Sharklasers.com email to confirm your account."));
        }
    }
}
