using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace VirtualGlassesProvider.Tests
{
    [TestFixture, Order(7)]
    internal class OrderTest
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
            Driver.Dispose();
        }


        [Test, Order(1)]
        public void ClientPlacesOrder()
        {
            Driver.Navigate().GoToUrl(AppServer.URL);
            Driver.Manage().Window.Size = new System.Drawing.Size(Display.DesktopWidth, Display.DesktopHeight);
            Driver.FindElement(By.Id("login")).Click();
            Driver.FindElement(By.Id("Input_Email")).SendKeys(TestClient.Email);
            Driver.FindElement(By.Id("Input_Password")).SendKeys(TestClient.Password);
            Driver.FindElement(By.Id("login-submit")).Click();
            Thread.Sleep(1000);
            var product = Driver.FindElement(By.CssSelector(".col-md-4:nth-child(1) .btn:nth-child(7)"));
            new Actions(Driver)
            .ScrollToElement(product)
            .Perform();
            var wait = new WebDriverWait(Driver, new TimeSpan(0, 0, 0, 10));
            var productElem = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(product));
            productElem.Click();
            Driver.FindElement(By.ClassName("fa")).Click();
            var checkout = Driver.FindElement(By.ClassName("btn-primary"));
            new Actions(Driver)
            .ScrollToElement(checkout)
            .Perform();
            var checkoutElem = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(checkout));
            checkoutElem.Click();
            Assert.That(Driver.FindElement(By.CssSelector(".card-title")).Text, Is.EqualTo("ORDER CONFIRMATION"));
        }
    }
}
