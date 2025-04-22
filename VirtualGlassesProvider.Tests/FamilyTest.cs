using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


namespace VirtualGlassesProvider.Tests
{
    [TestFixture, Order(8)]
    internal class FamilyTest
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
        public void ClientCancelsAddingFamily()
        {
            Driver.Navigate().GoToUrl(AppServer.URL);
            Driver.Manage().Window.Size = new System.Drawing.Size(Display.DesktopWidth, Display.DesktopHeight);
            Driver.FindElement(By.Id("login")).Click();
            Driver.FindElement(By.Id("Input_Email")).Click();
            Driver.FindElement(By.Id("Input_Email")).SendKeys(TestClient.Email);
            Driver.FindElement(By.Id("Input_Password")).Click();
            Driver.FindElement(By.Id("Input_Password")).SendKeys(TestClient.Password);
            Driver.FindElement(By.Id("login-submit")).Click();
            Driver.FindElement(By.Id("accountDashboard")).Click();
            Driver.FindElement(By.Id("manageFamily")).Click();
            Assert.That(Driver.FindElement(By.CssSelector("td")).Text, Is.EqualTo("No Family Added Yet"));
            Driver.FindElement(By.Id("family-form")).Click();
            var cancelBtn = Driver.FindElement(By.Id("exitFamilyFormBtn"));
            TestUtils.ClickElementSafely(ref cancelBtn, Driver);
            Assert.That(Driver.FindElement(By.CssSelector("td")).Text, Is.EqualTo("No Family Added Yet"));
        }


        [Test, Order(2)]
        public void ClientAddsAFamilyMember()
        {
            var memberFirstName = "Barack";
            var memberLastName = "Obama";
            var memberAddress = "122 Washington St";
            var memberEmail = "Barack@gmail.com";
            var memberRelationship = "Cousin";
            var memberPhone = "222-222-2222";
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
            Driver.FindElement(By.Id("manageFamily")).Click();
            Assert.Multiple(() =>
            {
                Assert.That(Driver.FindElement(By.CssSelector("td")).Text, Is.EqualTo("No Family Added Yet"));
                Driver.FindElement(By.Id("family-form")).Click();
                var altText = Driver.FindElement(By.Id("memberImage"))?.GetAttribute("alt")?.ToString();
                Assert.That(altText, Is.EqualTo("Placeholder Family Member Image"));
                Driver.FindElement(By.Id("Input_FirstName")).SendKeys(memberFirstName);
                Driver.FindElement(By.Id("Input_LastName")).SendKeys(memberLastName);
                Driver.FindElement(By.Id("Input_Address")).SendKeys(memberAddress);
                Driver.FindElement(By.Id("Input_Email")).SendKeys(memberEmail);
                Driver.FindElement(By.Id("Input_Relationship")).SendKeys(memberRelationship);
                Driver.FindElement(By.Id("Input_PhoneNumber")).SendKeys(memberPhone); ;
                var upload_file = Driver.FindElement(By.Id("Input_Image"));
                var file_path = Path.Join(projectDir, @"Resources\Faces\Barack_Obama.jpg");
                var img_path = Path.GetFullPath(file_path).Replace("\\", "/").Replace("/bin", "").Replace("/Debug", "");
                upload_file.SendKeys(img_path);
                var saveBtn = Driver.FindElement(By.Id("saveFamilyFormBtn"));
                TestUtils.ClickElementSafely(ref saveBtn, Driver);
                Thread.Sleep(1000);
                var firstNameDisplayed = Driver.FindElement(By.CssSelector("body > div > main > div > div > div.col-md-9 > table > tbody > tr > td:nth-child(1)")).Text;
                var lastNameDisplayed = Driver.FindElement(By.CssSelector("body > div > main > div > div > div.col-md-9 > table > tbody > tr > td:nth-child(2)")).Text;
                var addressDisplayed = Driver.FindElement(By.CssSelector("body > div > main > div > div > div.col-md-9 > table > tbody > tr > td:nth-child(3)")).Text;
                var emailDisplayed = Driver.FindElement(By.CssSelector("body > div > main > div > div > div.col-md-9 > table > tbody > tr > td:nth-child(4)")).Text;
                var phoneDisplayed = Driver.FindElement(By.CssSelector("body > div > main > div > div > div.col-md-9 > table > tbody > tr > td:nth-child(5)")).Text;
                Thread.Sleep(1000);
                Assert.That(firstNameDisplayed, Is.EqualTo(memberFirstName));
                Assert.That(lastNameDisplayed, Is.EqualTo(memberLastName));
                Assert.That(addressDisplayed, Is.EqualTo(memberAddress));
                Assert.That(emailDisplayed, Is.EqualTo(memberEmail));
                Assert.That(phoneDisplayed, Is.EqualTo(memberPhone));
            });
        }


        [Test, Order(3)]
        public void ClientEditsAFamilyMember()
        {
            var memberFirstName = "Janet";
            var memberLastName = "Sinclair";
            var memberAddress = "123 West St";
            var memberEmail = "JanetS@gmail.com";
            var memberRelationship = "Sister";
            var memberPhone = "432-233-231";
            string runningDir = TestContext.CurrentContext.TestDirectory;
            string projectDir = Directory.GetParent(runningDir)?.Parent?.FullName
                ?? throw new InvalidOperationException("Unable to determine the project directory.");
            Driver.Navigate().GoToUrl(AppServer.URL);
            Driver.Manage().Window.Size = new System.Drawing.Size(Display.DesktopWidth, Display.DesktopHeight);
            Driver.FindElement(By.Id("register")).Click();
            Driver.FindElement(By.Id("login")).Click();
            Driver.FindElement(By.Id("Input_Email")).SendKeys(TestClient.Email);
            Driver.FindElement(By.Id("Input_Password")).SendKeys(TestClient.Password);
            Driver.FindElement(By.Id("login-submit")).Click();
            Driver.FindElement(By.Id("accountDashboard")).Click();
            Driver.FindElement(By.Id("manageFamily")).Click();
            var editBtn = Driver.FindElement(By.CssSelector("body > div > main > div > div > div.col-md-9 > table > tbody > tr:nth-child(1) > td:nth-child(6) > a"));
            TestUtils.ClickElementSafely(ref editBtn, Driver);
            var altText = Driver.FindElement(By.Id("memberImage"))?.GetAttribute("alt")?.ToString();
            Assert.Multiple(() =>
            {
                Assert.That(altText, Is.EqualTo("Family Member Image"));
                Driver.FindElement(By.Id("Input_FirstName")).Clear();
                Driver.FindElement(By.Id("Input_FirstName")).SendKeys(memberFirstName);
                Driver.FindElement(By.Id("Input_LastName")).Clear();
                Driver.FindElement(By.Id("Input_LastName")).SendKeys(memberLastName);
                Driver.FindElement(By.Id("Input_Address")).Clear();
                Driver.FindElement(By.Id("Input_Address")).SendKeys(memberAddress);
                Driver.FindElement(By.Id("Input_Email")).Clear();
                Driver.FindElement(By.Id("Input_Email")).SendKeys(memberEmail);
                Driver.FindElement(By.Id("Input_Relationship")).Clear();
                Driver.FindElement(By.Id("Input_Relationship")).SendKeys(memberRelationship);
                Driver.FindElement(By.Id("Input_PhoneNumber")).Clear();
                Driver.FindElement(By.Id("Input_PhoneNumber")).SendKeys(memberPhone);
                var upload_file = Driver.FindElement(By.Id("Input_Image"));
                var file_path = Path.Join(projectDir, @"Resources\Faces\Janet_Sinclair.jpg");
                var img_path = Path.GetFullPath(file_path).Replace("\\", "/").Replace("/bin", "").Replace("/Debug", "");
                upload_file.SendKeys(img_path);
                var saveBtn = Driver.FindElement(By.Id("saveFamilyFormBtn"));
                TestUtils.ClickElementSafely(ref saveBtn, Driver);
                var firstNameDisplayed = Driver.FindElement(By.CssSelector("body > div > main > div > div > div.col-md-9 > table > tbody > tr > td:nth-child(1)")).Text;
                var lastNameDisplayed = Driver.FindElement(By.CssSelector("body > div > main > div > div > div.col-md-9 > table > tbody > tr > td:nth-child(2)")).Text;
                var addressDisplayed = Driver.FindElement(By.CssSelector("body > div > main > div > div > div.col-md-9 > table > tbody > tr > td:nth-child(3)")).Text;
                var emailDisplayed = Driver.FindElement(By.CssSelector("body > div > main > div > div > div.col-md-9 > table > tbody > tr > td:nth-child(4)")).Text;
                var phoneDisplayed = Driver.FindElement(By.CssSelector("body > div > main > div > div > div.col-md-9 > table > tbody > tr > td:nth-child(5)")).Text;
                Thread.Sleep(1000);
                Assert.That(firstNameDisplayed, Is.EqualTo(memberFirstName));
                Assert.That(lastNameDisplayed, Is.EqualTo(memberLastName));
                Assert.That(addressDisplayed, Is.EqualTo(memberAddress));
                Assert.That(emailDisplayed, Is.EqualTo(memberEmail));
                Assert.That(phoneDisplayed, Is.EqualTo(memberPhone));
            });
        }


        [Test, Order(4)]
        public void ClientGeneratesARImageForFamilyMember()
        {
            Driver.Navigate().GoToUrl(AppServer.URL);
            Driver.Manage().Window.Size = new System.Drawing.Size(Display.DesktopWidth, Display.DesktopHeight);
            Driver.FindElement(By.Id("login")).Click();
            Driver.FindElement(By.Id("Input_Email")).SendKeys(TestClient.Email);
            Driver.FindElement(By.Id("Input_Password")).SendKeys(TestClient.Password);
            Driver.FindElement(By.Id("login-submit")).Click();
            var product1DetailsBtn = Driver.FindElement(By.Id("productDetailsButton1"));
            TestUtils.ClickElementSafely(ref product1DetailsBtn, Driver);
            var buyForSelectList = Driver.FindElement(By.Id("buyFor"));
            TestUtils.ClickElementSafely(ref buyForSelectList, Driver);
            buyForSelectList.FindElement(By.XPath("//option[. = 'Janet Sinclair']")).Click();
            Driver.FindElement(By.CssSelector("#buyFor > option:nth-child(2)")).Click();
            var preRenderAltText = Driver.FindElement(By.ClassName("detailsImage"))?.GetAttribute("alt")?.ToString();
            Assert.That(preRenderAltText?.Equals("Render"), Is.False);
            var generateImageBtn = Driver.FindElement(By.Id("generateImageBtn"));
            TestUtils.ClickElementSafely(ref generateImageBtn, Driver);
            Thread.Sleep(5000);
            var renderAltText = Driver.FindElement(By.ClassName("detailsImage"))?.GetAttribute("alt")?.ToString();
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
            var revertedAltText = Driver.FindElement(By.ClassName("detailsImage"))?.GetAttribute("alt")?.ToString();
            Assert.That(revertedAltText, Is.EqualTo(preRenderAltText));
        }


        [Test, Order(5)]
        public void ClientDeletesFamilyMember()
        {
            Driver.Navigate().GoToUrl(AppServer.URL);
            Driver.Manage().Window.Size = new System.Drawing.Size(Display.DesktopWidth, Display.DesktopHeight);
            Driver.FindElement(By.Id("login")).Click();
            Driver.FindElement(By.Id("Input_Email")).SendKeys(TestClient.Email);
            Driver.FindElement(By.Id("Input_Password")).SendKeys(TestClient.Password);
            Driver.FindElement(By.Id("login-submit")).Click();
            Driver.FindElement(By.Id("accountDashboard")).Click();
            Driver.FindElement(By.Id("manageFamily")).Click();
            var deleteBtn = Driver.FindElement(By.CssSelector("body > div > main > div > div > div.col-md-9 > table > tbody > tr:nth-child(1) > td:nth-child(6) > form > input.btn.btn-danger"));
            TestUtils.ClickElementSafely(ref deleteBtn, Driver);
            Assert.That(Driver.FindElement(By.CssSelector("td")).Text, Is.EqualTo("No Family Added Yet"));
        }
    }
}
