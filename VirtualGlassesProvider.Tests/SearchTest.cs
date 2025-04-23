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
            Assert.That(Driver.FindElement(By.CssSelector("html body div.container main.pb-3 div.row div.col-md-4 div.card div.card-body h5.card-title")).Text, Is.EqualTo("RAYBAN"));
        }


        [Test, Order(2)]
        public void ClientSearchesByColour()
        {
            Driver.Navigate().GoToUrl(AppServer.URL);
            Driver.Manage().Window.Size = new System.Drawing.Size(Display.DesktopWidth, Display.DesktopHeight);
            Driver.FindElement(By.Id("searchString")).SendKeys("Blue");
            TestUtils.ClickElementSafely("searchBtn", Driver);
            Assert.That(Driver.FindElement(By.CssSelector(".col-md-4:nth-child(1) .label-text:nth-child(5)")).Text, Is.EqualTo("Colour: Blue"));
        }


        [Test, Order(3)]
        public void ClientSearchesByPrice()
        {
            Driver.Navigate().GoToUrl(AppServer.URL);
            Driver.Manage().Window.Size = new System.Drawing.Size(Display.DesktopWidth, Display.DesktopHeight);
            Driver.FindElement(By.Id("searchString")).SendKeys("10.99");
            TestUtils.ClickElementSafely("searchBtn", Driver);
            Assert.That(Driver.FindElement(By.CssSelector(".col-md-4:nth-child(1) .price")).Text, Does.Contain("$10.99"));
        }


        [Test, Order(4)]
        public void ClientSearchesByStyle()
        {
            Driver.Navigate().GoToUrl(AppServer.URL);
            Driver.Manage().Window.Size = new System.Drawing.Size(Display.DesktopWidth, Display.DesktopHeight);
            Driver.FindElement(By.Id("searchString")).SendKeys("Square");
            TestUtils.ClickElementSafely("searchBtn", Driver);
            Assert.That(Driver.FindElement(By.CssSelector(".col-md-4:nth-child(1) .label-text:nth-child(4)")).Text, Is.EqualTo("Style: Square"));
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
