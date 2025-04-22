using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


namespace VirtualGlassesProvider.Tests
{
    [TestFixture, Order(9)]
    internal class WishlistTest
    {
        private ChromeDriver Driver { get; set; }


        [SetUp]
        public void SetUp()
        {
            var options = new ChromeOptions { AcceptInsecureCertificates = true };
            //options.AddArgument("--headless=new");
            Driver = new ChromeDriver(options);
        }


        [TearDown]
        protected void TearDown()
        {
            Driver.Quit();
            Driver.Dispose();
        }


        [Test, Order(1)]
        public void ClientAddsItemsToWishlist()
        {
            Driver.Navigate().GoToUrl(AppServer.URL);
            Driver.Manage().Window.Size = new System.Drawing.Size(Display.DesktopWidth, Display.DesktopHeight);
            Driver.FindElement(By.Id("login")).Click();
            Driver.FindElement(By.Id("Input_Email")).SendKeys(TestClient.Email);
            Driver.FindElement(By.Id("Input_Password")).SendKeys(TestClient.Password);
            Driver.FindElement(By.Id("login-submit")).Click();
            Driver.FindElement(By.Id("navLinkWishlist")).Click();
            Assert.That(Driver.FindElement(By.CssSelector(".center")).Text, Is.EqualTo("There are no glasses on your wish list"));
            Driver.FindElement(By.Id("navLinkHome")).Click();
            var product1 = Driver.FindElement(By.Id("addToWishlistButton1"));
            TestUtils.ClickElementSafely(ref product1, Driver);
            Assert.That(Driver.FindElement(By.Id("addedToWishlistMessage")).Text, Is.EqualTo("Glasses successfully added to wishlist!"));
            var product2 = Driver.FindElement(By.Id("addToWishlistButton2"));
            TestUtils.ClickElementSafely(ref product2, Driver);
            Assert.That(Driver.FindElement(By.Id("addedToWishlistMessage")).Text, Is.EqualTo("Glasses successfully added to wishlist!"));
            product1 = Driver.FindElement(By.Id("addToWishlistButton1"));
            TestUtils.ClickElementSafely(ref product1, Driver);
            Assert.That(Driver.FindElement(By.Id("addedToWishlistMessage")).Text, Is.EqualTo("Glasses are already in the wishlist!"));
            Driver.FindElement(By.Id("navLinkWishlist")).Click();
            Assert.That(Driver.FindElement(By.LinkText("Rayban Black Colour Squared shaped Rayban Sunglasses")).Text, Is.EqualTo("Rayban Black Colour Squared shaped Rayban Sunglasses"));
            Driver.FindElement(By.LinkText("Rayban Black Colour Squared shaped Rayban Sunglasses")).Click();
            Assert.That(Driver.FindElement(By.CssSelector("h1")).Text, Is.EqualTo("RAYBAN BLACK SQUARE"));
        }


        [Test, Order(2)]
        public void ClientRemovesItemsFromWishlist()
        {
            Driver.Navigate().GoToUrl(AppServer.URL);
            Driver.Manage().Window.Size = new System.Drawing.Size(Display.DesktopWidth, Display.DesktopHeight);
            Driver.FindElement(By.Id("login")).Click();
            Driver.FindElement(By.Id("Input_Email")).SendKeys(TestClient.Email);
            Driver.FindElement(By.Id("Input_Password")).SendKeys(TestClient.Password);
            Driver.FindElement(By.Id("login-submit")).Click();
            Driver.FindElement(By.Id("navLinkWishlist")).Click();
            Driver.FindElement(By.Id("product1WishlistRemoveButton")).Click();
            Driver.FindElement(By.Id("product2WishlistRemoveButton")).Click();
            Assert.That(Driver.FindElement(By.ClassName("center")).Text, Is.EqualTo("There are no glasses on your wish list"));
        }
    }
}
