using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;


namespace VirtualGlassesProvider.Tests
{
    [TestFixture, Order(5)]
    internal class ARTest
    {
        #pragma warning disable NUnit1032
        private ChromeDriver _driver { get; set; }
        #pragma warning restore NUnit1032
        private string _downloadPath = Path.GetTempPath();

        [SetUp]
        public void SetUp()
        {
            ChromeOptions options = new ChromeOptions { AcceptInsecureCertificates = true };
            options.AddUserProfilePreference("download.default_directory", _downloadPath);
            options.AddUserProfilePreference("download.prompt_for_download", false);
            options.AddUserProfilePreference("disable-popup-blocking", "true");
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
            _driver.Navigate().GoToUrl(AppServer.URL);
            _driver.Manage().Window.Size = new System.Drawing.Size(Display.DesktopWidth, Display.DesktopHeight);
            var product = _driver.FindElement(By.CssSelector(".col-md-4:nth-child(1) .btn-primary"));
            new Actions(_driver)
            .ScrollToElement(product)
            .Perform();
            WebDriverWait wait = new WebDriverWait(_driver, new TimeSpan(0, 0, 0, 10));
            var elem = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(product));
            elem.Click();
            _driver.FindElement(By.Id("generateImageBtn")).Click();
            Thread.Sleep(1000);
            Assert.That(_driver.FindElement(By.CssSelector(".errorMessage")).Text, Is.EqualTo("Please login to use this feature"));
        }


        [Test, Order(2)]
        public void AuthenticatedUserGeneratesImageForSelfSuccessfully()
        {
            _driver.Navigate().GoToUrl(AppServer.URL);
            _driver.Manage().Window.Size = new System.Drawing.Size(Display.DesktopWidth, Display.DesktopHeight);
            _driver.FindElement(By.Id("login")).Click();
            _driver.FindElement(By.Id("Input_Email")).SendKeys(TestClient.Email);
            _driver.FindElement(By.Id("Input_Password")).SendKeys(TestClient.Password);
            _driver.FindElement(By.Id("login-submit")).Click();
            var product = _driver.FindElement(By.CssSelector(".col-md-4:nth-child(1) .btn-primary"));
            new Actions(_driver)
            .ScrollToElement(product)
            .Perform();
            WebDriverWait wait = new WebDriverWait(_driver, new TimeSpan(0, 0, 0, 10));
            var productElement = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(product));
            productElement.Click();
            var preRenderAltText = _driver.FindElement(By.Id("detailsImage")).GetAttribute("alt").ToString();
            Assert.That(preRenderAltText.Equals("Render"), Is.EqualTo(false));
            Thread.Sleep(5000);
            _driver.FindElement(By.Id("generateImageBtn")).Click();
            Thread.Sleep(5000);
            var renderAltText = _driver.FindElement(By.Id("detailsImage")).GetAttribute("alt").ToString();
            Assert.That(renderAltText, Is.EqualTo("Render"));
            var downloadImage = _driver.FindElement(By.Id("downloadImageLink"));
            new Actions(_driver)
            .ScrollToElement(downloadImage)
            .Perform();
            var downloadImageElement = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(downloadImage));
            downloadImageElement.Click();
            Thread.Sleep(5000);
            var fileName = $"ARGeneratedImage.jpg";
            var file = Directory.GetFiles(_downloadPath, fileName, SearchOption.TopDirectoryOnly);
            Assert.That(file != null);
            File.Delete(file[0]);
            _driver.FindElement(By.Id("clearImage")).Click();
            var revertedAltText = _driver.FindElement(By.ClassName("detailsImage")).GetAttribute("alt").ToString();
            Assert.That(revertedAltText, Is.EqualTo(preRenderAltText));
        }
    }
}
