using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;


namespace VirtualGlassesProvider.Tests
{
    [TestFixture, Order(8)]
    internal class FamilyTest
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
        public void ClientCancelsAddingFamily()
        {
            _driver.Navigate().GoToUrl("https://localhost:7044/");
            _driver.Manage().Window.Size = new System.Drawing.Size(1012, 691);
            _driver.FindElement(By.Id("login")).Click();
            _driver.FindElement(By.Id("Input_Email")).Click();
            _driver.FindElement(By.Id("Input_Email")).SendKeys(TestClient.Email);
            _driver.FindElement(By.Id("Input_Password")).Click();
            _driver.FindElement(By.Id("Input_Password")).SendKeys(TestClient.Password);
            _driver.FindElement(By.Id("login-submit")).Click();
            _driver.FindElement(By.Id("manage")).Click();
            _driver.FindElement(By.Id("family")).Click();
            Assert.That(_driver.FindElement(By.CssSelector("td")).Text, Is.EqualTo("No Family Added Yet"));
            _driver.FindElement(By.Id("family-form")).Click();
            var cancelBtn = _driver.FindElement(By.Id("exitFamilyFormBtn"));
            new Actions(_driver)
              .ScrollToElement(cancelBtn)
              .Perform();
            WebDriverWait wait = new WebDriverWait(_driver, new TimeSpan(0,0,0,10));
            IWebElement elem = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(cancelBtn));
            elem.Click();
            Assert.That(_driver.FindElement(By.CssSelector("td")).Text, Is.EqualTo("No Family Added Yet"));
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
            string projectDir = Directory.GetParent(runningDir).Parent.FullName;
            _driver.Navigate().GoToUrl("https://localhost:7044/");
            _driver.Manage().Window.Size = new System.Drawing.Size(1012, 691);
            _driver.FindElement(By.Id("login")).Click();
            _driver.FindElement(By.Id("Input_Email")).Click();
            _driver.FindElement(By.Id("Input_Email")).SendKeys(TestClient.Email);
            _driver.FindElement(By.Id("Input_Password")).Click();
            _driver.FindElement(By.Id("Input_Password")).SendKeys(TestClient.Password);
            _driver.FindElement(By.Id("login-submit")).Click();
            _driver.FindElement(By.Id("manage")).Click();
            _driver.FindElement(By.Id("family")).Click();
            Assert.That(_driver.FindElement(By.CssSelector("td")).Text, Is.EqualTo("No Family Added Yet"));
            _driver.FindElement(By.Id("family-form")).Click();
            var altText = _driver.FindElement(By.Id("memberImage")).GetAttribute("alt").ToString();
            Assert.That(altText, Is.EqualTo("Placeholder Family Member Image"));
            _driver.FindElement(By.Id("Input_FirstName")).SendKeys(memberFirstName);
            _driver.FindElement(By.Id("Input_LastName")).SendKeys(memberLastName);
            _driver.FindElement(By.Id("Input_Address")).SendKeys(memberAddress);
            _driver.FindElement(By.Id("Input_Email")).SendKeys(memberEmail);
            _driver.FindElement(By.Id("Input_Relationship")).SendKeys(memberRelationship);
            _driver.FindElement(By.Id("Input_PhoneNumber")).SendKeys(memberPhone);;
            var upload_file = _driver.FindElement(By.Id("Input_Image"));
            var file_path = Path.Join(projectDir, @"Resources\Faces\Barack_Obama.jpg");
            var img_path = Path.GetFullPath(file_path).Replace("\\", "/").Replace("/bin", "").Replace("/Debug", "");
            upload_file.SendKeys(img_path);
            var saveBtn = _driver.FindElement(By.Id("saveFamilyFormBtn"));
            new Actions(_driver)
             .ScrollToElement(saveBtn)
             .Perform();
            WebDriverWait wait = new WebDriverWait(_driver, new TimeSpan(0,0,0,10));
            IWebElement elem = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(saveBtn));
            elem.Click();
            Thread.Sleep(1000);
            var firstNameDisplayed = _driver.FindElement(By.CssSelector("body > div > main > div > div > div.col-md-9 > table > tbody > tr > td:nth-child(1)")).Text;
            var lastNameDisplayed = _driver.FindElement(By.CssSelector("body > div > main > div > div > div.col-md-9 > table > tbody > tr > td:nth-child(2)")).Text;
            var addressDisplayed = _driver.FindElement(By.CssSelector("body > div > main > div > div > div.col-md-9 > table > tbody > tr > td:nth-child(3)")).Text;
            var emailDisplayed = _driver.FindElement(By.CssSelector("body > div > main > div > div > div.col-md-9 > table > tbody > tr > td:nth-child(4)")).Text;
            var phoneDisplayed = _driver.FindElement(By.CssSelector("body > div > main > div > div > div.col-md-9 > table > tbody > tr > td:nth-child(5)")).Text;
            Thread.Sleep(1000);
            Assert.That(firstNameDisplayed, Is.EqualTo(memberFirstName));
            Assert.That(lastNameDisplayed, Is.EqualTo(memberLastName));
            Assert.That(addressDisplayed, Is.EqualTo(memberAddress));
            Assert.That(emailDisplayed, Is.EqualTo(memberEmail));
            Assert.That(phoneDisplayed, Is.EqualTo(memberPhone));
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
            string projectDir = Directory.GetParent(runningDir).Parent.FullName;
            _driver.Navigate().GoToUrl("https://localhost:7044/");
            _driver.Manage().Window.Size = new System.Drawing.Size(1012, 991);
            _driver.FindElement(By.Id("register")).Click();
            _driver.FindElement(By.Id("login")).Click();
            _driver.FindElement(By.Id("Input_Email")).Click();
            _driver.FindElement(By.Id("Input_Email")).SendKeys(TestClient.Email);
            _driver.FindElement(By.Id("Input_Password")).Click();
            _driver.FindElement(By.Id("Input_Password")).SendKeys(TestClient.Password);
            _driver.FindElement(By.Id("login-submit")).Click();
            _driver.FindElement(By.Id("manage")).Click();
            _driver.FindElement(By.Id("family")).Click();
            var editBtn = _driver.FindElement(By.CssSelector("body > div > main > div > div > div.col-md-9 > table > tbody > tr:nth-child(1) > td:nth-child(6) > a"));
            new Actions(_driver)
            .ScrollToElement(editBtn)
            .Perform();
            editBtn.Click();
            var altText = _driver.FindElement(By.Id("memberImage")).GetAttribute("alt").ToString();
            Assert.That(altText, Is.EqualTo("Family Member Image"));
            _driver.FindElement(By.Id("Input_FirstName")).Clear();
            _driver.FindElement(By.Id("Input_FirstName")).SendKeys(memberFirstName);
            _driver.FindElement(By.Id("Input_LastName")).Clear();
            _driver.FindElement(By.Id("Input_LastName")).SendKeys(memberLastName);
            _driver.FindElement(By.Id("Input_Address")).Clear();
            _driver.FindElement(By.Id("Input_Address")).SendKeys(memberAddress);
            _driver.FindElement(By.Id("Input_Email")).Clear();
            _driver.FindElement(By.Id("Input_Email")).SendKeys(memberEmail);
            _driver.FindElement(By.Id("Input_Relationship")).Clear();
            _driver.FindElement(By.Id("Input_Relationship")).SendKeys(memberRelationship);
            _driver.FindElement(By.Id("Input_PhoneNumber")).Clear();
            _driver.FindElement(By.Id("Input_PhoneNumber")).SendKeys(memberPhone);
            var upload_file = _driver.FindElement(By.Id("Input_Image"));
            var file_path = Path.Join(projectDir, @"Resources\Faces\Janet_Sinclair.jpg");
            var img_path = Path.GetFullPath(file_path).Replace("\\", "/").Replace("/bin", "").Replace("/Debug", "");
            upload_file.SendKeys(img_path);
            var saveBtn = _driver.FindElement(By.Id("saveFamilyFormBtn"));
            new Actions(_driver)
             .ScrollToElement(saveBtn)
             .Perform();
            WebDriverWait wait = new WebDriverWait(_driver, new TimeSpan(0,0,0,10));
            var elem = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(saveBtn));
            elem.Click();
            var firstNameDisplayed = _driver.FindElement(By.CssSelector("body > div > main > div > div > div.col-md-9 > table > tbody > tr > td:nth-child(1)")).Text;
            var lastNameDisplayed = _driver.FindElement(By.CssSelector("body > div > main > div > div > div.col-md-9 > table > tbody > tr > td:nth-child(2)")).Text;
            var addressDisplayed = _driver.FindElement(By.CssSelector("body > div > main > div > div > div.col-md-9 > table > tbody > tr > td:nth-child(3)")).Text;
            var emailDisplayed = _driver.FindElement(By.CssSelector("body > div > main > div > div > div.col-md-9 > table > tbody > tr > td:nth-child(4)")).Text;
            var phoneDisplayed = _driver.FindElement(By.CssSelector("body > div > main > div > div > div.col-md-9 > table > tbody > tr > td:nth-child(5)")).Text;
            Thread.Sleep(1000);
            Assert.That(firstNameDisplayed, Is.EqualTo(memberFirstName));
            Assert.That(lastNameDisplayed, Is.EqualTo(memberLastName));
            Assert.That(addressDisplayed, Is.EqualTo(memberAddress));
            Assert.That(emailDisplayed, Is.EqualTo(memberEmail));
            Assert.That(phoneDisplayed, Is.EqualTo(memberPhone));
        }


        [Test, Order(4)]
        public void ClientGeneratesARImageForFamilyMember()
        {
            _driver.Navigate().GoToUrl("https://localhost:7044/");
            _driver.Manage().Window.Size = new System.Drawing.Size(1012, 691);
            _driver.FindElement(By.Id("login")).Click();
            _driver.FindElement(By.Id("Input_Email")).Click();
            _driver.FindElement(By.Id("Input_Email")).SendKeys(TestClient.Email);
            _driver.FindElement(By.Id("Input_Password")).Click();
            _driver.FindElement(By.Id("Input_Password")).SendKeys(TestClient.Password);
            _driver.FindElement(By.Id("login-submit")).Click();
            var product = _driver.FindElement(By.CssSelector(".col-md-4:nth-child(1) .btn-primary"));
            new Actions(_driver)
            .ScrollToElement(product)
            .Perform();
            WebDriverWait wait = new WebDriverWait(_driver, new TimeSpan(0, 0, 0, 10));
            var elem = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(product));
            elem.Click();
            _driver.FindElement(By.Id("buyFor")).Click();
            {
                var dropdown = _driver.FindElement(By.Id("buyFor"));
                dropdown.FindElement(By.XPath("//option[. = 'Janet Sinclair']")).Click();
            }
            _driver.FindElement(By.CssSelector("#buyFor > option:nth-child(2)")).Click();
            var preRenderAltText = _driver.FindElement(By.ClassName("detailsImage")).GetAttribute("alt").ToString();
            Assert.That(preRenderAltText.Equals("Render"), Is.EqualTo(false));
            _driver.FindElement(By.Id("generateImageBtn")).Click();
            Thread.Sleep(5000);
            var renderAltText = _driver.FindElement(By.ClassName("detailsImage")).GetAttribute("alt").ToString();
            Assert.That(renderAltText, Is.EqualTo("Render"));
            _driver.FindElement(By.Id("downloadImageLink")).Click();
            Thread.Sleep(5000);
            var fileName = $"ARGeneratedImage.jpg";
            var file = Directory.GetFiles(_downloadPath, fileName, SearchOption.TopDirectoryOnly);
            Assert.That(file != null);
            File.Delete(file[0]);
            _driver.FindElement(By.Id("clearImage")).Click();
            var revertedAltText = _driver.FindElement(By.ClassName("detailsImage")).GetAttribute("alt").ToString();
            Assert.That(revertedAltText, Is.EqualTo(preRenderAltText));
        }


        [Test, Order(5)]
        public void ClientDeletesFamilyMember()
        {
            _driver.Navigate().GoToUrl("https://localhost:7044/");
            _driver.Manage().Window.Size = new System.Drawing.Size(1012, 991);
            _driver.FindElement(By.Id("login")).Click();
            _driver.FindElement(By.Id("Input_Email")).Click();
            _driver.FindElement(By.Id("Input_Email")).SendKeys(TestClient.Email);
            _driver.FindElement(By.Id("Input_Password")).Click();
            _driver.FindElement(By.Id("Input_Password")).SendKeys(TestClient.Password);
            _driver.FindElement(By.Id("login-submit")).Click();
            _driver.FindElement(By.Id("manage")).Click();
            _driver.FindElement(By.Id("family")).Click();
            var deleteBtn =_driver.FindElement(By.CssSelector("body > div > main > div > div > div.col-md-9 > table > tbody > tr:nth-child(1) > td:nth-child(6) > form > input.btn.btn-danger"));
            new Actions(_driver)
           .ScrollToElement(deleteBtn)
           .Perform();
            deleteBtn.Click();
            Assert.That(_driver.FindElement(By.CssSelector("td")).Text, Is.EqualTo("No Family Added Yet"));
        }
    }
}
