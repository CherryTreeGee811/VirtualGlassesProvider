using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


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
            TestUtils.ClickElementSafely("login", Driver);
            Driver.FindElement(By.Id("Input_Email")).SendKeys(TestClient.Email);
            Driver.FindElement(By.Id("Input_Password")).SendKeys(TestClient.Password);
            TestUtils.ClickElementSafely("login-submit", Driver);
            TestUtils.WaitForElementToBeVisible(By.Id("addToCartButton1"), Driver);
            TestUtils.ClickElementSafely("addToCartButton1", Driver);
            TestUtils.ClickElementSafely("ViewCartButton", Driver);
            TestUtils.ClickElementSafely("cartCheckoutButton", Driver);
            Assert.That(Driver.FindElement(By.Id("orderConfirmationPageTitle")).Text, Is.EqualTo("ORDER CONFIRMATION"));
        }
    }
}
