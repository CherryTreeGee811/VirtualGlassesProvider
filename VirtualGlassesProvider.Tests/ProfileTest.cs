using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


namespace VirtualGlassesProvider.Tests
{
    [TestFixture, Order(4)]
    internal class ProfileTest
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
        public void ClientConfiguresProfile()
        {
            string runningDir = TestContext.CurrentContext.TestDirectory;
            string projectDir = Directory.GetParent(runningDir).Parent.FullName;
            _driver.Navigate().GoToUrl("https://localhost:7044/");
            _driver.Manage().Window.Size = new System.Drawing.Size(1936, 1056);
            _driver.FindElement(By.Id("login")).Click();
            _driver.FindElement(By.Id("Input_Email")).Click();
            _driver.FindElement(By.Id("Input_Email")).SendKeys("TestClient@Sharklasers.com");
            _driver.FindElement(By.Id("Input_Password")).Click();
            _driver.FindElement(By.Id("Input_Password")).SendKeys("Test1$");
            _driver.FindElement(By.Id("login-submit")).Click();
            _driver.FindElement(By.Id("manage")).Click();
            var altText = _driver.FindElement(By.CssSelector("#profileImage")).GetAttribute("alt").ToString();
            Assert.That(altText, Is.EqualTo("Placeholder Profile Image"));
            _driver.FindElement(By.Id("Input_DisplayName")).Click();
            _driver.FindElement(By.Id("Input_DisplayName")).SendKeys("Tim Apple");
            var upload_file = _driver.FindElement(By.Id("Input_Image"));
            var file_path = Path.Join(projectDir, @"..\Resources\Faces\tim_apple.jpg");
            var img_path = Path.GetFullPath(file_path).Replace("\\", "/");
            upload_file.SendKeys(img_path);
            _driver.FindElement(By.CssSelector(".btn-primary")).Click();
            Thread.Sleep(1000);
            Assert.That(_driver.FindElement(By.CssSelector(".alert")).Text, Is.EqualTo("Your profile has been updated"));
            altText = _driver.FindElement(By.CssSelector("#profileImage")).GetAttribute("alt").ToString();
            Assert.That(altText, Is.EqualTo("Profile Image"));
        }
    }
}
