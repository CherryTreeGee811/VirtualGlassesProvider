using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


namespace VirtualGlassesProvider.Tests
{
    [TestFixture, Order(3)]
    internal class SearchTest
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
        public void ClientSearchesByBrandName()
        {
            Driver.Navigate().GoToUrl(AppServer.URL);
            Driver.Manage().Window.Size = new System.Drawing.Size(Display.DesktopWidth, Display.DesktopHeight);
            Driver.FindElement(By.Id("searchString")).SendKeys("Rayban");
            TestUtils.ClickElementSafely("searchBtn", Driver);
            Assert.That(Driver.FindElement(By.Id("searchProduct1BrandName")).Text, Is.EqualTo("RAYBAN"));
        }


        [Test, Order(2)]
        public void ClientSearchesByColour()
        {
            Driver.Navigate().GoToUrl(AppServer.URL);
            Driver.Manage().Window.Size = new System.Drawing.Size(Display.DesktopWidth, Display.DesktopHeight);
            Driver.FindElement(By.Id("searchString")).SendKeys("Blue");
            TestUtils.ClickElementSafely("searchBtn", Driver);
            Assert.That(Driver.FindElement(By.CssSelector("searchProduct2Colour")).Text, Is.EqualTo("Colour: Blue"));
        }


        [Test, Order(3)]
        public void ClientSearchesByPrice()
        {
            Driver.Navigate().GoToUrl(AppServer.URL);
            Driver.Manage().Window.Size = new System.Drawing.Size(Display.DesktopWidth, Display.DesktopHeight);
            Driver.FindElement(By.Id("searchString")).SendKeys("10.99");
            TestUtils.ClickElementSafely("searchBtn", Driver);
            Assert.That(Driver.FindElement(By.Id("searchProduct1Price")).Text, Does.Contain("$10.99"));
        }


        [Test, Order(4)]
        public void ClientSearchesByStyle()
        {
            Driver.Navigate().GoToUrl(AppServer.URL);
            Driver.Manage().Window.Size = new System.Drawing.Size(Display.DesktopWidth, Display.DesktopHeight);
            Driver.FindElement(By.Id("searchString")).SendKeys("Square");
            TestUtils.ClickElementSafely("searchBtn", Driver);
            Assert.That(Driver.FindElement(By.Id("searchProduct1Style")).Text, Is.EqualTo("Style: Square"));
        }


        [Test, Order(5)]
        public void ClientFailsToFindSearchResults()
        {
            Driver.Navigate().GoToUrl(AppServer.URL);
            Driver.Manage().Window.Size = new System.Drawing.Size(Display.DesktopWidth, Display.DesktopHeight);
            Driver.FindElement(By.Id("searchString")).SendKeys("!~~");
            TestUtils.ClickElementSafely("searchBtn", Driver);
            Assert.That(Driver.FindElement(By.CssSelector("h1")).Text, Is.EqualTo("NO RESULTS FOUND :("));
        }
    }
}
