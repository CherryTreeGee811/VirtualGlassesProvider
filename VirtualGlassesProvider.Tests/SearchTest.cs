using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;


namespace VirtualGlassesProvider.Tests
{
    [TestFixture, Order(3)]
    internal class SearchTest
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
        public void ClientSearchesByBrandName()
        {
            _driver.Navigate().GoToUrl(AppServer.URL);
            _driver.Manage().Window.Size = new System.Drawing.Size(Display.DesktopWidth, Display.DesktopHeight);
            _driver.FindElement(By.Id("searchString")).SendKeys("Rayban");
            var searchBtn = _driver.FindElement(By.CssSelector(".btn:nth-child(3)"));
            new Actions(_driver)
              .ScrollToElement(searchBtn)
              .Perform();
            WebDriverWait wait = new WebDriverWait(_driver, new TimeSpan(0, 0, 0, 10));
            IWebElement searchBtnElem = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(searchBtn));
            searchBtnElem.Click();
            Assert.That(_driver.FindElement(By.CssSelector("html body div.container main.pb-3 div.row div.col-md-4 div.card div.card-body h5.card-title")).Text, Is.EqualTo("RAYBAN"));
        }


        [Test, Order(2)]
        public void ClientSearchesByColour()
        {
            _driver.Navigate().GoToUrl(AppServer.URL);
            _driver.Manage().Window.Size = new System.Drawing.Size(Display.DesktopWidth, Display.DesktopHeight);
            _driver.FindElement(By.Id("searchString")).SendKeys("Blue");
            var searchBtn = _driver.FindElement(By.CssSelector(".btn:nth-child(3)"));
            new Actions(_driver)
              .ScrollToElement(searchBtn)
              .Perform();
            WebDriverWait wait = new WebDriverWait(_driver, new TimeSpan(0, 0, 0, 10));
            IWebElement searchBtnElem = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(searchBtn));
            searchBtnElem.Click();
            Assert.That(_driver.FindElement(By.CssSelector(".col-md-4:nth-child(1) .label-text:nth-child(5)")).Text, Is.EqualTo("Colour: Blue"));
        }


        [Test, Order(3)]
        public void ClientSearchesByPrice()
        {
            _driver.Navigate().GoToUrl(AppServer.URL);
            _driver.Manage().Window.Size = new System.Drawing.Size(Display.DesktopWidth, Display.DesktopHeight);
            _driver.FindElement(By.Id("searchString")).SendKeys("10.99");
            var searchBtn = _driver.FindElement(By.CssSelector(".btn:nth-child(3)"));
            new Actions(_driver)
              .ScrollToElement(searchBtn)
              .Perform();
            WebDriverWait wait = new WebDriverWait(_driver, new TimeSpan(0, 0, 0, 10));
            IWebElement searchBtnElem = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(searchBtn));
            searchBtnElem.Click();
            _driver.FindElement(By.CssSelector(".btn:nth-child(3)")).Click();
            Assert.That(_driver.FindElement(By.CssSelector(".col-md-4:nth-child(1) .price")).Text.Contains("$10.99"), Is.True);
        }


        [Test, Order(4)]
        public void ClientSearchesByStyle()
        {
            _driver.Navigate().GoToUrl(AppServer.URL);
            _driver.Manage().Window.Size = new System.Drawing.Size(Display.DesktopWidth, Display.DesktopHeight);
            _driver.FindElement(By.Id("searchString")).SendKeys("Square");
            var searchBtn = _driver.FindElement(By.CssSelector(".btn:nth-child(3)"));
            new Actions(_driver)
              .ScrollToElement(searchBtn)
              .Perform();
            WebDriverWait wait = new WebDriverWait(_driver, new TimeSpan(0, 0, 0, 10));
            IWebElement searchBtnElem = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(searchBtn));
            searchBtnElem.Click();
            Assert.That(_driver.FindElement(By.CssSelector(".col-md-4:nth-child(1) .label-text:nth-child(4)")).Text, Is.EqualTo("Style: Square"));
        }


        [Test, Order(5)]
        public void ClientFailsToFindSearchResults()
        {
            _driver.Navigate().GoToUrl(AppServer.URL);
            _driver.Manage().Window.Size = new System.Drawing.Size(Display.DesktopWidth, Display.DesktopHeight);
            _driver.FindElement(By.Id("searchString")).SendKeys("!~~");
            var searchBtn = _driver.FindElement(By.CssSelector(".btn:nth-child(3)"));
            new Actions(_driver)
              .ScrollToElement(searchBtn)
              .Perform();
            WebDriverWait wait = new WebDriverWait(_driver, new TimeSpan(0, 0, 0, 10));
            IWebElement searchBtnElem = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(searchBtn));
            searchBtnElem.Click();
            Assert.That(_driver.FindElement(By.CssSelector("h1")).Text, Is.EqualTo("NO RESULTS FOUND :("));
        }
    }
}
