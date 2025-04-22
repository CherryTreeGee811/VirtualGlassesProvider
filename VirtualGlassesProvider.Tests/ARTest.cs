using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


namespace VirtualGlassesProvider.Tests
{
    [TestFixture, Order(5)]
    internal class ARTest
    {
        private ChromeDriver Driver { get; set; }
        private readonly string _downloadPath = Path.GetTempPath();

        [SetUp]
        public void SetUp()
        {
            var options = new ChromeOptions { AcceptInsecureCertificates = true };
            options.AddUserProfilePreference("download.default_directory", _downloadPath);
            options.AddUserProfilePreference("download.prompt_for_download", false);
            options.AddUserProfilePreference("disable-popup-blocking", "true");
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
        public void UnauthenticatedUserClicksGenerateImage()
        {
            Driver.Navigate().GoToUrl(AppServer.URL);
            Driver.Manage().Window.Size = new System.Drawing.Size(Display.DesktopWidth, Display.DesktopHeight);
            var product = Driver.FindElement(By.Id("productDetailsButton1"));
            TestUtils.ClickElementSafely(ref product, Driver);
            var generateImageBtn = Driver.FindElement(By.Id("generateImageBtn"));
            TestUtils.ClickElementSafely(ref generateImageBtn, Driver);
            Thread.Sleep(1000);
            Assert.That(Driver.FindElement(By.Id("detailsErrorMessage")).Text, Is.EqualTo("Please login to use this feature"));
        }


        [Test, Order(2)]
        public void AuthenticatedUserGeneratesImageForSelfSuccessfully()
        {
            Driver.Navigate().GoToUrl(AppServer.URL);
            Driver.Manage().Window.Size = new System.Drawing.Size(Display.DesktopWidth, Display.DesktopHeight);
            Driver.FindElement(By.Id("login")).Click();
            Driver.FindElement(By.Id("Input_Email")).SendKeys(TestClient.Email);
            Driver.FindElement(By.Id("Input_Password")).SendKeys(TestClient.Password);
            Driver.FindElement(By.Id("login-submit")).Click();
            var product = Driver.FindElement(By.Id("productDetailsButton1"));
            TestUtils.ClickElementSafely(ref product, Driver);
            var preRenderAltText = Driver.FindElement(By.Id("detailsImage"))?.GetAttribute("alt")?.ToString();
            Assert.That(preRenderAltText?.Equals("Render"), Is.False);
            Thread.Sleep(5000);
            var generateImageBtn = Driver.FindElement(By.Id("generateImageBtn"));
            TestUtils.ClickElementSafely(ref generateImageBtn, Driver);
            Thread.Sleep(5000);
            var renderAltText = Driver.FindElement(By.Id("detailsImage"))?.GetAttribute("alt")?.ToString();
            Assert.That(renderAltText, Is.EqualTo("Render"));
            var downloadImage = Driver.FindElement(By.Id("downloadImageLink"));
            TestUtils.ClickElementSafely(ref downloadImage, Driver);
            Thread.Sleep(5000);
            var fileName = $"ARGeneratedImage.jpg";
            var file = Directory.GetFiles(_downloadPath, fileName, SearchOption.TopDirectoryOnly);
            Assert.That(file, Is.Not.Null);
            if (file != null && file.Length > 0)
            {
                File.Delete(file[0]);
            }
            Driver.FindElement(By.Id("clearImage")).Click();
            var revertedAltText = Driver.FindElement(By.Id("detailsImage"))?.GetAttribute("alt")?.ToString();
            Assert.That(revertedAltText, Is.EqualTo(preRenderAltText));
        }
    }
}
