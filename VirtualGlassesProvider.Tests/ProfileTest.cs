using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;


namespace VirtualGlassesProvider.Tests
{
    [TestFixture, Order(4)]
    internal class ProfileTest
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
        public void ClientConfiguresProfile()
        {
            const string initialFirstName = "Tim";
            const string initialLastName = "Apple";
            const string initialPhoneNumber = "461-821-5721";
            const string initialAddress = "123 West St";
            const string initialDisplayName = "Tim Apple";
            string runningDir = TestContext.CurrentContext.TestDirectory;
            string projectDir = Directory.GetParent(runningDir)?.Parent?.FullName
                ?? throw new InvalidOperationException("Unable to determine the project directory.");
            Driver.Navigate().GoToUrl(AppServer.URL);
            Driver.Manage().Window.Size = new System.Drawing.Size(Display.DesktopWidth, Display.DesktopHeight);
            Driver.FindElement(By.Id("login")).Click();
            Driver.FindElement(By.Id("Input_Email")).SendKeys(TestClient.Email);
            Driver.FindElement(By.Id("Input_Password")).SendKeys(TestClient.Password);
            Driver.FindElement(By.Id("login-submit")).Click();
            Driver.FindElement(By.Id("accountDashboard")).Click();
            Driver.FindElement(By.Id("manageProfile")).Click();
            var altText = Driver.FindElement(By.Id("profileImage"))?.GetAttribute("alt")?.ToString();
            Assert.Multiple(() =>
            {
                Assert.That(altText, Is.EqualTo("Placeholder Profile Image"));
                Driver.FindElement(By.Id("Input_FirstName")).SendKeys(initialFirstName);
                Driver.FindElement(By.Id("Input_LastName")).SendKeys(initialLastName);
                Driver.FindElement(By.Id("Input_PhoneNumber")).SendKeys(initialPhoneNumber);
                Driver.FindElement(By.Id("Input_Address")).SendKeys(initialAddress);
                Driver.FindElement(By.Id("Input_DisplayName")).SendKeys(initialDisplayName);
                var upload_file = Driver.FindElement(By.Id("Input_Image"));
                var file_path = Path.Join(projectDir, @"Resources\Faces\tim_apple.jpg");
                var img_path = Path.GetFullPath(file_path).Replace("\\", "/").Replace("/bin", "").Replace("/Debug", "");
                upload_file.SendKeys(img_path);
                var submitDetails = Driver.FindElement(By.ClassName("btn-primary"));
                new Actions(Driver)
                .ScrollToElement(submitDetails)
                .Perform();
                var wait = new WebDriverWait(Driver, new TimeSpan(0, 0, 0, 10));
                var submitDetailsElem = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(submitDetails));
                submitDetailsElem.Click();
                Thread.Sleep(1000);
                Assert.That(Driver.FindElement(By.ClassName("alert")).Text, Is.EqualTo("Your profile has been updated"));
                var firstNameSubmitted = Driver.FindElement(By.Id("Input_FirstName"))?.GetAttribute("value")?.ToString();
                var lastNameSubmitted = Driver.FindElement(By.Id("Input_LastName"))?.GetAttribute("value")?.ToString();
                var phoneNumberSubmitted = Driver.FindElement(By.Id("Input_PhoneNumber"))?.GetAttribute("value")?.ToString();
                var addressSubmitted = Driver.FindElement(By.Id("Input_Address"))?.GetAttribute("value")?.ToString();
                var displayNameSubmitted = Driver.FindElement(By.Id("Input_DisplayName"))?.GetAttribute("value")?.ToString();
                var newAltText = Driver.FindElement(By.Id("profileImage"))?.GetAttribute("alt")?.ToString();
                Assert.That(firstNameSubmitted, Is.EqualTo(initialFirstName));
                Assert.That(lastNameSubmitted, Is.EqualTo(initialLastName));
                Assert.That(phoneNumberSubmitted, Is.EqualTo(initialPhoneNumber));
                Assert.That(addressSubmitted, Is.EqualTo(initialAddress));
                Assert.That(displayNameSubmitted, Is.EqualTo(initialDisplayName));
                Assert.That(newAltText, Is.EqualTo("Profile Image"));
            });
        }
    }
}
