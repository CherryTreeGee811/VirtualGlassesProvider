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
            const string initialFirstName = "Tim";
            const string initialLastName = "Apple";
            const string initialPhoneNumber = "461-821-5721";
            const string initialAddress = "123 West St";
            const string initialDisplayName = "Tim Apple";
            string runningDir = TestContext.CurrentContext.TestDirectory;
            string projectDir = Directory.GetParent(runningDir).Parent.FullName;
            _driver.Navigate().GoToUrl(AppServer.URL);
            _driver.Manage().Window.Size = new System.Drawing.Size(1936, 1056);
            _driver.FindElement(By.Id("login")).Click();
            _driver.FindElement(By.Id("Input_Email")).SendKeys(TestClient.Email);
            _driver.FindElement(By.Id("Input_Password")).SendKeys(TestClient.Password);
            _driver.FindElement(By.Id("login-submit")).Click();
            _driver.FindElement(By.Id("accountDashboard")).Click();
            _driver.FindElement(By.Id("manageProfile")).Click();
            var altText = _driver.FindElement(By.Id("profileImage")).GetAttribute("alt").ToString();
            Assert.That(altText, Is.EqualTo("Placeholder Profile Image"));
            _driver.FindElement(By.Id("Input_FirstName")).SendKeys(initialFirstName);
            _driver.FindElement(By.Id("Input_LastName")).SendKeys(initialLastName);
            _driver.FindElement(By.Id("Input_PhoneNumber")).SendKeys(initialPhoneNumber);
            _driver.FindElement(By.Id("Input_Address")).SendKeys(initialAddress);
            _driver.FindElement(By.Id("Input_DisplayName")).SendKeys(initialDisplayName);
            var upload_file = _driver.FindElement(By.Id("Input_Image"));
            var file_path = Path.Join(projectDir, @"Resources\Faces\tim_apple.jpg");
            var img_path = Path.GetFullPath(file_path).Replace("\\", "/").Replace("/bin", "").Replace("/Debug", "");
            upload_file.SendKeys(img_path);
            _driver.FindElement(By.ClassName("btn-primary")).Click();
            Thread.Sleep(1000);
            Assert.That(_driver.FindElement(By.ClassName("alert")).Text, Is.EqualTo("Your profile has been updated"));
            var firstNameSubmitted = _driver.FindElement(By.Id("Input_FirstName")).GetAttribute("value").ToString();
            var lastNameSubmitted = _driver.FindElement(By.Id("Input_LastName")).GetAttribute("value").ToString();
            var phoneNumberSubmitted = _driver.FindElement(By.Id("Input_PhoneNumber")).GetAttribute("value").ToString();
            var addressSubmitted = _driver.FindElement(By.Id("Input_Address")).GetAttribute("value").ToString();
            var displayNameSubmitted = _driver.FindElement(By.Id("Input_DisplayName")).GetAttribute("value").ToString();
            var newAltText = _driver.FindElement(By.Id("profileImage")).GetAttribute("alt").ToString();
            Assert.That(firstNameSubmitted, Is.EqualTo(initialFirstName));
            Assert.That(lastNameSubmitted, Is.EqualTo(initialLastName));
            Assert.That(phoneNumberSubmitted, Is.EqualTo(initialPhoneNumber));
            Assert.That(addressSubmitted, Is.EqualTo(initialAddress));
            Assert.That(displayNameSubmitted, Is.EqualTo(initialDisplayName));
            Assert.That(newAltText, Is.EqualTo("Profile Image"));
        }
    }
}
