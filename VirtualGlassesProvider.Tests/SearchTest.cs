using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Text.RegularExpressions;



namespace VirtualGlassesProvider.Tests
{
    [TestFixture, Order(3)]
    internal class SearchTest
    {
        #pragma warning disable NUnit1032
        private static ChromeDriver s_driver { get; set; }
        #pragma warning restore NUnit1032


        [SetUp]
        public static void SetUp()
        {
            ChromeOptions options = new ChromeOptions { AcceptInsecureCertificates = true };
            options.AddArgument("--headless=new");
            s_driver = new ChromeDriver(options);
        }


        [TearDown]
        protected static void TearDown()
        {
            s_driver.Quit();
        }


        [Test, Order(1)]
        public static void ClientSearchesByBrandName()
        {
            s_driver.Navigate().GoToUrl("https://localhost:7044/");
            s_driver.Manage().Window.Size = new System.Drawing.Size(1012, 991);
            s_driver.FindElement(By.Id("searchString")).Click();
            s_driver.FindElement(By.Id("searchString")).SendKeys("Rayban");
            s_driver.FindElement(By.CssSelector(".btn:nth-child(3)")).Click();
            Assert.That(s_driver.FindElement(By.CssSelector("html body div.container main.pb-3 div.row div.col-md-4 div.card div.card-body h5.card-title")).Text, Is.EqualTo("RAYBAN"));
        }


        [Test, Order(2)]
        public static void ClientSearchesByColour()
        {
            s_driver.Navigate().GoToUrl("https://localhost:7044/");
            s_driver.Manage().Window.Size = new System.Drawing.Size(1012, 991);
            s_driver.FindElement(By.Id("searchString")).Click();
            s_driver.FindElement(By.Id("searchString")).SendKeys("Blue");
            s_driver.FindElement(By.CssSelector(".btn:nth-child(3)")).Click();
            Assert.That(s_driver.FindElement(By.CssSelector(".card-text:nth-child(5)")).Text, Is.EqualTo("Colour: Blue"));
        }


        [Test, Order(3)]
        public static void ClientSearchesByPrice()
        {
            s_driver.Navigate().GoToUrl("https://localhost:7044/");
            s_driver.Manage().Window.Size = new System.Drawing.Size(1012, 691);
            s_driver.FindElement(By.Id("searchString")).Click();
            s_driver.FindElement(By.Id("searchString")).SendKeys("10.99");
            s_driver.FindElement(By.CssSelector(".btn:nth-child(3)")).Click();
            Assert.That(s_driver.FindElement(By.CssSelector(".col-md-4:nth-child(1) .card-text:nth-child(3)")).Text.Contains("10.99"), Is.True);
        }


        [Test, Order(4)]
        public static void ClientSearchesByStyle()
        {
            s_driver.Navigate().GoToUrl("https://localhost:7044/");
            s_driver.Manage().Window.Size = new System.Drawing.Size(1012, 691);
            s_driver.FindElement(By.Id("searchString")).Click();
            s_driver.FindElement(By.Id("searchString")).SendKeys("Square");
            s_driver.FindElement(By.CssSelector(".btn:nth-child(3)")).Click();
            Assert.That(s_driver.FindElement(By.CssSelector(".card-text:nth-child(4)")).Text, Is.EqualTo("Style: Square"));
        }


        [Test, Order(5)]
        public static void ClientFailsToFindSearchResults()
        {
            s_driver.Navigate().GoToUrl("https://localhost:7044/");
            s_driver.Manage().Window.Size = new System.Drawing.Size(1012, 691);
            s_driver.FindElement(By.Id("searchString")).Click();
            s_driver.FindElement(By.Id("searchString")).SendKeys("!~~");
            s_driver.FindElement(By.CssSelector(".btn:nth-child(3)")).Click();
            Assert.That(s_driver.FindElement(By.CssSelector("h1")).Text, Is.EqualTo("NO RESULTS FOUND :("));
        }
    }
}
