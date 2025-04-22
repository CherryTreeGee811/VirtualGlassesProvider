using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


namespace VirtualGlassesProvider.Tests
{
    [TestFixture, Order(2)]
    internal class LoginTest
    {
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
            Driver.Dispose(); ;
        }


        [Test, Order(1)]
        public void ClientFailsToLogin()
        {
            Driver.Navigate().GoToUrl(AppServer.URL);
            Driver.Manage().Window.Size = new System.Drawing.Size(Display.DesktopWidth, Display.DesktopHeight);
            Driver.FindElement(By.Id("login")).Click();
            Driver.FindElement(By.Id("Input_Email")).SendKeys(TestClient.Email);
            Driver.FindElement(By.Id("Input_Password")).SendKeys("Test1S");
            Driver.FindElement(By.Id("login-submit")).Click();
            Assert.That(Driver.FindElement(By.CssSelector(".text-danger li")).Text, Is.EqualTo("Invalid login attempt."));
        }


        [Test, Order(2)]
        public void ClientLogsInSuccessfully()
        {
            Driver.Navigate().GoToUrl(AppServer.URL);
            Driver.Manage().Window.Size = new System.Drawing.Size(Display.DesktopWidth, Display.DesktopHeight);
            Driver.FindElement(By.Id("login")).Click();
            Driver.FindElement(By.Id("Input_Email")).SendKeys(TestClient.Email);
            Driver.FindElement(By.Id("Input_Password")).SendKeys(TestClient.Password);
            Driver.FindElement(By.Id("login-submit")).Click();
            Assert.That(string.Equals(Driver.FindElement(By.Id("accountDashboard")).Text, TestClient.Email.ToUpper()));
        }
    }
}
