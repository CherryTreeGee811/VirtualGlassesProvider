using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace VirtualGlassesProvider.Tests
{
    [TestFixture, Order(10)]
    internal class CartTest
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
        public void UnauthenticatedUserTrysToUseCartFeature()
        {
            _driver.Navigate().GoToUrl(AppServer.URL);
            _driver.Manage().Window.Size = new System.Drawing.Size(Display.DesktopWidth, Display.DesktopHeight);
            _driver.FindElement(By.ClassName("fa")).Click();
            Assert.That(_driver.FindElement(By.CssSelector("h1")).Text, Is.EqualTo("LOG IN"));
            _driver.FindElement(By.CssSelector(".flex-grow-1 > .nav-item:nth-child(1) > .nav-link")).Click();
            WebDriverWait wait = new WebDriverWait(_driver, new TimeSpan(0, 0, 0, 10));
            var product = _driver.FindElement(By.CssSelector(".col-md-4:nth-child(1) .btn:nth-child(7)"));
            new Actions(_driver)
            .ScrollToElement(product)
           .Perform();
            var productElem = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(product));
            productElem.Click();
            Assert.That(_driver.FindElement(By.CssSelector("h1")).Text, Is.EqualTo("LOG IN"));
        }


        [Test, Order(2)]
        public void ClientAddsAndRemovesItemsInCart()
        {
            _driver.Navigate().GoToUrl(AppServer.URL);
            _driver.Manage().Window.Size = new System.Drawing.Size(Display.DesktopWidth, Display.DesktopHeight);
            _driver.FindElement(By.Id("login")).Click();
            _driver.FindElement(By.Id("Input_Email")).SendKeys(TestClient.Email);
            _driver.FindElement(By.Id("Input_Password")).SendKeys(TestClient.Password);
            _driver.FindElement(By.Id("login-submit")).Click();
            ClientAddsItemsToCart();
            ClientRemovesItemsFromCart();
        }


        private void ClientAddsItemsToCart()
        {
            _driver.FindElement(By.ClassName("fa")).Click();
            Assert.That(_driver.FindElement(By.CssSelector("h3")).Text, Is.EqualTo("YOUR CART IS EMPTY."));
            _driver.FindElement(By.CssSelector(".flex-grow-1 > .nav-item:nth-child(1) > .nav-link")).Click();
            var product1BrandName = _driver.FindElement(By.CssSelector(".col-md-4:nth-child(1) .card-title")).Text.ToUpper();
            var product1Description = _driver.FindElement(By.CssSelector(".col-md-4:nth-child(1) .card-text")).Text.ToUpper();
            var product2BrandName = _driver.FindElement(By.CssSelector(".col-md-4:nth-child(2) .card-title")).Text.ToUpper();
            var product2Description = _driver.FindElement(By.CssSelector(".col-md-4:nth-child(2) .card-text")).Text.ToUpper();
            var product1Button = _driver.FindElement(By.CssSelector(".col-md-4:nth-child(1) .btn:nth-child(7)"));
            WebDriverWait wait = new WebDriverWait(_driver, new TimeSpan(0, 0, 0, 10));
            new Actions(_driver)
            .ScrollToElement(product1Button)
            .Perform();
            var product1ButtonElem = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(product1Button));
            product1ButtonElem.Click();
            Assert.That(_driver.FindElement(By.Id("addedToCartMessage")).Text, Is.EqualTo("Glasses successfully added to cart!"));
            Assert.That(_driver.FindElement(By.ClassName("fa")).Text, Is.EqualTo("1"));
            var product2Button = _driver.FindElement(By.CssSelector(".col-md-4:nth-child(2) .btn-primary"));
            new Actions(_driver)
            .ScrollToElement(product2Button)
            .Perform();
            var product2ButtonElem = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(product2Button));
            product2ButtonElem.Click();
            _driver.FindElement(By.CssSelector(".btn-dark:nth-child(6)")).Click();
            Assert.That(_driver.FindElement(By.Id("addedToCartMessage")).Text, Is.EqualTo("Glasses successfully added to cart!"));
            Assert.That(_driver.FindElement(By.ClassName("fa")).Text, Is.EqualTo("2"));
            _driver.FindElement(By.CssSelector(".flex-grow-1 > .nav-item:nth-child(1) > .nav-link")).Click();
            product1Button = _driver.FindElement(By.CssSelector(".col-md-4:nth-child(1) .btn:nth-child(7)"));
            new Actions(_driver)
            .ScrollToElement(product1Button)
            .Perform();
            product1ButtonElem = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(product1Button));
            product1ButtonElem.Click();
            Assert.That(_driver.FindElement(By.ClassName("fa")).Text, Is.EqualTo("3"));
            _driver.FindElement(By.ClassName("fa")).Click();
            Assert.That(_driver.FindElement(By.CssSelector(".col-12:nth-child(1) .card-title")).Text, Is.EqualTo($"{product1BrandName} - {product1Description}"));
            Assert.That(_driver.FindElement(By.CssSelector(".col-12:nth-child(1) .card-text:nth-child(2)")).Text, Is.EqualTo("Quantity: 2"));
            Assert.That(_driver.FindElement(By.CssSelector(".col-12:nth-child(2) .card-title")).Text, Is.EqualTo($"{product2BrandName} - {product2Description}"));
            Assert.That(_driver.FindElement(By.CssSelector(".col-12:nth-child(2) .card-text:nth-child(2)")).Text, Is.EqualTo("Quantity: 1"));
        }


        private void ClientRemovesItemsFromCart()
        {
            _driver.FindElement(By.CssSelector(".col-12:nth-child(1) .btn")).Click();
            Assert.That(_driver.FindElement(By.ClassName("fa")).Text, Is.EqualTo("2"));
            _driver.FindElement(By.CssSelector(".col-12:nth-child(2) .btn")).Click();
            Assert.That(_driver.FindElement(By.ClassName("fa")).Text, Is.EqualTo("1"));
            _driver.FindElement(By.CssSelector(".col-12:nth-child(1) .btn")).Click();
            Assert.That(_driver.FindElement(By.CssSelector("h3")).Text, Is.EqualTo("YOUR CART IS EMPTY."));
        }
    }
}
