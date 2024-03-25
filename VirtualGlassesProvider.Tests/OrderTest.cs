using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace VirtualGlassesProvider.Tests
{
    [TestFixture, Order(7)]
    internal class OrderTest
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
        public void ClientPlacesOrder()
        {
            _driver.Navigate().GoToUrl("https://localhost:7044/");
            _driver.Manage().Window.Size = new System.Drawing.Size(1936, 1056);
            _driver.FindElement(By.Id("login")).Click();
            _driver.FindElement(By.Id("Input_Email")).Click();
            _driver.FindElement(By.Id("Input_Email")).SendKeys(TestClient.Email);
            _driver.FindElement(By.Id("Input_Password")).Click();
            _driver.FindElement(By.Id("Input_Password")).SendKeys(TestClient.Password);
            _driver.FindElement(By.Id("login-submit")).Click();
            Thread.Sleep(1000);
            _driver.FindElement(By.CssSelector("body > div > main > div:nth-child(5) > div:nth-child(1) > div > div.card-body > a.btn.btn-success")).Click();
            _driver.FindElement(By.CssSelector(".fa")).Click();
            _driver.FindElement(By.ClassName("btn-primary")).Click();
            Assert.That(_driver.FindElement(By.CssSelector(".card-title")).Text, Is.EqualTo("ORDER CONFIRMATION"));
        }
    }
}
