using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;


namespace VirtualGlassesProvider.Tests
{
    [TestFixture, Order(9)]
    internal class WishlistTest
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
        public void UnauthenticatedUserTrysToUseWishlistFeature()
        {
            _driver.Navigate().GoToUrl(AppServer.URL);
            _driver.Manage().Window.Size = new System.Drawing.Size(Display.DesktopWidth, Display.DesktopHeight);
            _driver.FindElement(By.CssSelector(".col-md-4:nth-child(1) .btn:nth-child(9)")).Click();
            Assert.That(_driver.FindElement(By.CssSelector("h1")).Text, Is.EqualTo("LOG IN"));
            _driver.FindElement(By.CssSelector(".flex-grow-1 > .nav-item:nth-child(1) > .nav-link")).Click();
            _driver.FindElement(By.CssSelector(".nav-item:nth-child(4) > .nav-link")).Click();
            Assert.That(_driver.FindElement(By.CssSelector("h1")).Text, Is.EqualTo("LOG IN"));
        }


        [Test, Order(2)]
        public void ClientAddsItemsToWishlist()
        {
            _driver.Navigate().GoToUrl(AppServer.URL);
            _driver.Manage().Window.Size = new System.Drawing.Size(Display.DesktopWidth, Display.DesktopHeight);
            _driver.FindElement(By.Id("login")).Click();
            _driver.FindElement(By.Id("Input_Email")).SendKeys(TestClient.Email);
            _driver.FindElement(By.Id("Input_Password")).SendKeys(TestClient.Password);
            _driver.FindElement(By.Id("login-submit")).Click();
            _driver.FindElement(By.CssSelector(".nav-item:nth-child(4) > .nav-link")).Click();
            Assert.That(_driver.FindElement(By.CssSelector(".center")).Text, Is.EqualTo("There are no glasses on your wish list"));
            _driver.FindElement(By.CssSelector(".nav-item:nth-child(1) > .nav-link")).Click();
            var product1 = _driver.FindElement(By.CssSelector(".col-md-4:nth-child(1) .btn:nth-child(9)"));
            new Actions(_driver)
            .ScrollToElement(product1)
            .Perform();
            WebDriverWait wait = new WebDriverWait(_driver, new TimeSpan(0, 0, 0, 10));
            var product1Elem = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(product1));
            product1Elem.Click();
            Assert.That(_driver.FindElement(By.Id("addedToWishlistMessage")).Text, Is.EqualTo("Glasses successfully added to wishlist!"));
            var product2 = _driver.FindElement(By.CssSelector(".col-md-4:nth-child(2) .btn:nth-child(9)"));
            new Actions(_driver)
           .ScrollToElement(product2)
           .Perform();
            var product2Elem = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(product2));
            product2Elem.Click();
            Assert.That(_driver.FindElement(By.Id("addedToWishlistMessage")).Text, Is.EqualTo("Glasses successfully added to wishlist!"));
            product1 = _driver.FindElement(By.CssSelector(".col-md-4:nth-child(1) .btn:nth-child(9)"));
            new Actions(_driver)
            .ScrollToElement(product1)
            .Perform();
            product1Elem = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(product1));
            product1Elem.Click();
            Assert.That(_driver.FindElement(By.Id("addedToWishlistMessage")).Text, Is.EqualTo("Glasses are already in the wishlist!"));
            _driver.FindElement(By.CssSelector(".nav-item:nth-child(4) > .nav-link")).Click();
            Assert.That(_driver.FindElement(By.LinkText("Rayban Black Colour Squared shaped Rayban Sunglasses")).Text, Is.EqualTo("Rayban Black Colour Squared shaped Rayban Sunglasses"));
            _driver.FindElement(By.LinkText("Rayban Black Colour Squared shaped Rayban Sunglasses")).Click();
            Assert.That(_driver.FindElement(By.CssSelector("h1")).Text, Is.EqualTo("RAYBAN BLACK SQUARE"));
        }


        [Test, Order(3)]
        public void ClientRemovesItemsFromWishlist()
        {
            _driver.Navigate().GoToUrl(AppServer.URL);
            _driver.Manage().Window.Size = new System.Drawing.Size(Display.DesktopWidth, Display.DesktopHeight);
            _driver.FindElement(By.Id("login")).Click();
            _driver.FindElement(By.Id("Input_Email")).SendKeys(TestClient.Email);
            _driver.FindElement(By.Id("Input_Password")).SendKeys(TestClient.Password);
            _driver.FindElement(By.Id("login-submit")).Click();
            _driver.FindElement(By.CssSelector(".nav-item:nth-child(4) > .nav-link")).Click();
            _driver.FindElement(By.CssSelector("tr:nth-child(2) .btn")).Click();
            _driver.FindElement(By.ClassName("btn-danger")).Click();
            Assert.That(_driver.FindElement(By.ClassName("center")).Text, Is.EqualTo("There are no glasses on your wish list"));
        }
    }
}
