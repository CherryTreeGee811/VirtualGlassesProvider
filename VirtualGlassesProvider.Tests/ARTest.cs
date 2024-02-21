using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


namespace VirtualGlassesProvider.Tests
{
    [TestFixture, Order(5)]
    internal class ARTest
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
        public void UnauthenticatedUserClicksGenerateImage()
        {
            _driver.Navigate().GoToUrl("https://localhost:7044/");
            _driver.Manage().Window.Size = new System.Drawing.Size(1012, 991);
            _driver.FindElement(By.CssSelector(".col-md-4:nth-child(1) .btn-primary")).Click();
            _driver.FindElement(By.Id("generateImageBtn")).Click();
            Thread.Sleep(1000);
            Assert.That(_driver.FindElement(By.CssSelector(".errorMessage")).Text, Is.EqualTo("Please login to use this feature"));
        }


        [Test, Order(2)]
        public void AuthenticatedUserGeneratesImageSuccessfully()
        {
            _driver.Navigate().GoToUrl("https://localhost:7044/");
            _driver.Manage().Window.Size = new System.Drawing.Size(1012, 991);
            _driver.FindElement(By.Id("login")).Click();
            _driver.FindElement(By.Id("Input_Email")).Click();
            _driver.FindElement(By.Id("Input_Email")).SendKeys("TestClient@Sharklasers.com");
            _driver.FindElement(By.Id("Input_Password")).Click();
            _driver.FindElement(By.Id("login-submit")).Click();
            _driver.FindElement(By.Id("Input_Password")).SendKeys("Test1$");
            _driver.FindElement(By.Id("login-submit")).Click();
            _driver.FindElement(By.CssSelector(".col-md-4:nth-child(1) .btn-primary")).Click();
            var altText = _driver.FindElement(By.CssSelector(".detailsImage")).GetAttribute("alt").ToString();
            Assert.That(altText.Equals("Render"), Is.EqualTo(false));
            _driver.FindElement(By.Id("generateImageBtn")).Click();
            Thread.Sleep(3000);
            altText = _driver.FindElement(By.CssSelector(".detailsImage")).GetAttribute("alt").ToString();
            Assert.That(altText, Is.EqualTo("Render"));
        }
    }
}
