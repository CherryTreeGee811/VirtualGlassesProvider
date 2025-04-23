using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


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
                Assert.That(Driver.FindElement(By.Id("cartIsEmptyText")).Text, Is.EqualTo("YOUR CART IS EMPTY."));
                TestUtils.ClickElementSafely("navLinkHome", Driver);
                var product1BrandName = Driver.FindElement(By.Id("product1BrandName")).Text.ToUpper();
                var product1Description = Driver.FindElement(By.Id("product1Description")).Text.ToUpper();
                var product2BrandName = Driver.FindElement(By.Id("product2BrandName")).Text.ToUpper();
                var product2Description = Driver.FindElement(By.Id("product2Description")).Text.ToUpper();
                TestUtils.ClickElementSafely("addToCartButton1", Driver);
                Assert.That(Driver.FindElement(By.Id("addedToCartMessage")).Text, Is.EqualTo("Glasses successfully added to cart!"));
                Assert.That(Driver.FindElement(By.Id("ViewCartButton")).Text, Is.EqualTo("1"));
                TestUtils.ClickElementSafely("productDetailsButton2", Driver);
                Thread.Sleep(1000);
                TestUtils.ClickElementSafely("detailsAddToCartButton", Driver);
                Assert.That(Driver.FindElement(By.Id("addedToCartMessage")).Text, Is.EqualTo("Glasses successfully added to cart!"));
                Assert.That(Driver.FindElement(By.Id("ViewCartButton")).Text, Is.EqualTo("2"));
                TestUtils.ClickElementSafely("navLinkHome", Driver);
                TestUtils.ClickElementSafely("addToCartButton1", Driver);
                Assert.That(Driver.FindElement(By.Id("ViewCartButton")).Text, Is.EqualTo("3"));
                TestUtils.ClickElementSafely("ViewCartButton", Driver);
                Assert.That(Driver.FindElement(By.Id("cartProduct1BrandNameAndDescription")).Text, Is.EqualTo($"{product1BrandName} - {product1Description}"));
                Assert.That(Driver.FindElement(By.Id("cartProduct1Quantity")).Text, Is.EqualTo("Quantity: 2"));
                Assert.That(Driver.FindElement(By.Id("cartProduct2BrandNameAndDescription")).Text, Is.EqualTo($"{product2BrandName} - {product2Description}"));
                Assert.That(Driver.FindElement(By.Id("cartProduct2Quantity")).Text, Is.EqualTo("Quantity: 1"));
            });
        }


        private void ClientRemovesItemsFromCart()
        {
            TestUtils.ClickElementSafely("product1CartRemoveButton", Driver);
            Assert.That(Driver.FindElement(By.Id("ViewCartButton")).Text, Is.EqualTo("2"));
            TestUtils.ClickElementSafely("product2CartRemoveButton", Driver);
            Assert.That(Driver.FindElement(By.Id("ViewCartButton")).Text, Is.EqualTo("1"));
            TestUtils.ClickElementSafely("product1CartRemoveButton", Driver);
            Assert.That(Driver.FindElement(By.Id("cartIsEmptyText")).Text, Is.EqualTo("YOUR CART IS EMPTY."));
        }
    }
}
