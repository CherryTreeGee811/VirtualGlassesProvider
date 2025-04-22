using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace VirtualGlassesProvider.Tests
{
    [TestFixture, Order(10)]
    internal class CartTest
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
        public void ClientAddsAndRemovesItemsInCart()
        {
            Driver.Navigate().GoToUrl(AppServer.URL);
            Driver.Manage().Window.Size = new System.Drawing.Size(Display.DesktopWidth, Display.DesktopHeight);
            Driver.FindElement(By.Id("login")).Click();
            Driver.FindElement(By.Id("Input_Email")).SendKeys(TestClient.Email);
            Driver.FindElement(By.Id("Input_Password")).SendKeys(TestClient.Password);
            Driver.FindElement(By.Id("login-submit")).Click();
            ClientAddsItemsToCart();
            ClientRemovesItemsFromCart();
        }


        private void ClientAddsItemsToCart()
        {
            Driver.FindElement(By.Id("ViewCartButton")).Click();
            Assert.Multiple(() =>
            {
                Assert.That(Driver.FindElement(By.CssSelector("h3")).Text, Is.EqualTo("YOUR CART IS EMPTY."));
                Driver.FindElement(By.CssSelector(".flex-grow-1 > .nav-item:nth-child(1) > .nav-link")).Click();
                var product1BrandName = Driver.FindElement(By.CssSelector(".col-md-4:nth-child(1) .card-title")).Text.ToUpper();
                var product1Description = Driver.FindElement(By.CssSelector(".col-md-4:nth-child(1) .card-text")).Text.ToUpper();
                var product2BrandName = Driver.FindElement(By.CssSelector(".col-md-4:nth-child(2) .card-title")).Text.ToUpper();
                var product2Description = Driver.FindElement(By.CssSelector(".col-md-4:nth-child(2) .card-text")).Text.ToUpper();
                var product1Button = Driver.FindElement(By.CssSelector(".col-md-4:nth-child(1) .btn:nth-child(7)"));
                var wait = new WebDriverWait(Driver, new TimeSpan(0, 0, 0, 10));
                new Actions(Driver)
                .ScrollToElement(product1Button)
                .Perform();
                var product1ButtonElem = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(product1Button));
                product1ButtonElem.Click();
                Assert.That(Driver.FindElement(By.Id("addedToCartMessage")).Text, Is.EqualTo("Glasses successfully added to cart!"));
                Assert.That(Driver.FindElement(By.Id("ViewCartButton")).Text, Is.EqualTo("1"));
                var product2Button = Driver.FindElement(By.Id("productDetailsButton2"));
                new Actions(Driver)
                .ScrollToElement(product2Button)
                .Perform();
                var product2ButtonElem = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(product2Button));
                product2ButtonElem.Click();
                Thread.Sleep(1000);
                var addToCartBtn = Driver.FindElement(By.Id("detailsAddToCartButton"));
                new Actions(Driver)
                .ScrollToElement(addToCartBtn)
                .Perform();
                var addToCartBtnElem = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(addToCartBtn));
                addToCartBtnElem.Click();
                Assert.That(Driver.FindElement(By.Id("addedToCartMessage")).Text, Is.EqualTo("Glasses successfully added to cart!"));
                Assert.That(Driver.FindElement(By.Id("ViewCartButton")).Text, Is.EqualTo("2"));
                Driver.FindElement(By.CssSelector(".flex-grow-1 > .nav-item:nth-child(1) > .nav-link")).Click();
                product1Button = Driver.FindElement(By.CssSelector(".col-md-4:nth-child(1) .btn:nth-child(7)"));
                new Actions(Driver)
                .ScrollToElement(product1Button)
                .Perform();
                product1ButtonElem = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(product1Button));
                product1ButtonElem.Click();
                Assert.That(Driver.FindElement(By.Id("ViewCartButton")).Text, Is.EqualTo("3"));
                Driver.FindElement(By.Id("ViewCartButton")).Click();
                Assert.That(Driver.FindElement(By.CssSelector(".col-12:nth-child(1) .card-title")).Text, Is.EqualTo($"{product1BrandName} - {product1Description}"));
                Assert.That(Driver.FindElement(By.CssSelector(".col-12:nth-child(1) .card-text:nth-child(2)")).Text, Is.EqualTo("Quantity: 2"));
                Assert.That(Driver.FindElement(By.CssSelector(".col-12:nth-child(2) .card-title")).Text, Is.EqualTo($"{product2BrandName} - {product2Description}"));
                Assert.That(Driver.FindElement(By.CssSelector(".col-12:nth-child(2) .card-text:nth-child(2)")).Text, Is.EqualTo("Quantity: 1"));
            });
        }


        private void ClientRemovesItemsFromCart()
        {
            Driver.FindElement(By.Id("product1RemoveButton")).Click();
            Assert.That(Driver.FindElement(By.Id("ViewCartButton")).Text, Is.EqualTo("2"));
            var product2RemoveBtn = Driver.FindElement(By.Id("product2RemoveButton"));
            new Actions(Driver)
            .ScrollToElement(product2RemoveBtn)
            .Perform();
            var wait = new WebDriverWait(Driver, new TimeSpan(0, 0, 0, 10));
            var product2RemoveBtnElem = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(product2RemoveBtn));
            product2RemoveBtnElem.Click();
            Assert.That(Driver.FindElement(By.Id("ViewCartButton")).Text, Is.EqualTo("1"));
            Driver.FindElement(By.Id("product1RemoveButton")).Click();
            Assert.That(Driver.FindElement(By.CssSelector("h3")).Text, Is.EqualTo("YOUR CART IS EMPTY."));
        }
    }
}
