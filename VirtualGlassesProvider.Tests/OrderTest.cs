using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

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
            _driver.Navigate().GoToUrl(AppServer.URL);
            _driver.Manage().Window.Size = new System.Drawing.Size(Display.DesktopWidth, Display.DesktopHeight);
            _driver.FindElement(By.Id("login")).Click();
            _driver.FindElement(By.Id("Input_Email")).SendKeys(TestClient.Email);
            _driver.FindElement(By.Id("Input_Password")).SendKeys(TestClient.Password);
            _driver.FindElement(By.Id("login-submit")).Click();
            Thread.Sleep(1000);
            var product = _driver.FindElement(By.CssSelector(".col-md-4:nth-child(1) .btn:nth-child(7)"));
            new Actions(_driver)
            .ScrollToElement(product)
            .Perform();
            WebDriverWait wait = new WebDriverWait(_driver, new TimeSpan(0, 0, 0, 10));
            var productElem = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(product));
            productElem.Click();
            _driver.FindElement(By.ClassName("fa")).Click();
            var checkout = _driver.FindElement(By.ClassName("btn-primary"));
            new Actions(_driver)
            .ScrollToElement(checkout)
            .Perform();
            var checkoutElem = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(checkout));
            checkoutElem.Click();
            Assert.That(_driver.FindElement(By.CssSelector(".card-title")).Text, Is.EqualTo("ORDER CONFIRMATION"));
        }
    }
}
