using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


namespace VirtualGlassesProvider.Tests
{
    [TestFixture, Order(6)]
    internal class PreconfigurePaymentTest
    {
        private ChromeDriver Driver { get; set; }


        [SetUp]
        public void SetUp()
        {
            var options = new ChromeOptions { AcceptInsecureCertificates = true };
            options.AddArgument("--headless=new");
            options.AddUserProfilePreference("autofill.credit_card_enabled", false);
            Driver = new ChromeDriver(options);
        }


        [TearDown]
        protected void TearDown()
        {
            Driver.Quit();
            Driver.Dispose();
        }


        [Test, Order(1)]
        public void ClientDoesNotPreconfigurePayment()
        {
            Driver.Navigate().GoToUrl(AppServer.URL);
            Driver.Manage().Window.Size = new System.Drawing.Size(Display.DesktopWidth, Display.DesktopHeight);
            Driver.FindElement(By.Id("login")).Click();
            Driver.FindElement(By.Id("Input_Email")).SendKeys(TestClient.Email);
            Driver.FindElement(By.Id("Input_Password")).SendKeys(TestClient.Password);
            Driver.FindElement(By.Id("login-submit")).Click();
            var addProduct1ToCartBtn = Driver.FindElement(By.Id("addToCartButton1"));
            TestUtils.ClickElementSafely("addToCartButton1", Driver);
            Driver.FindElement(By.Id("ViewCartButton")).Click();
            var preconfiguredCardHolderName = Driver.FindElement(By.Id("CardHolderName"))?.GetAttribute("value")?.ToString();
            var preconfiguredCardNumber = Driver.FindElement(By.Id("CardNumber"))?.GetAttribute("value")?.ToString();
            var preconfiguredCvv = Driver.FindElement(By.Id("CVV"))?.GetAttribute("value")?.ToString();
            var preconfiguredExpiryDate = Driver.FindElement(By.Id("ExpiryDate"))?.GetAttribute("value")?.ToString();
            Assert.Multiple(() =>
            {
                Assert.That(preconfiguredCardHolderName, Is.EqualTo(""));
                Assert.That(preconfiguredCardNumber, Is.EqualTo(""));
                Assert.That(preconfiguredCvv, Is.EqualTo(""));
                Assert.That(preconfiguredExpiryDate, Is.EqualTo(""));
            });
        }


        [Test, Order(2)]
        public void ClientPreconfiguresPayment()
        {
            const string cardHolderName = "Tim Apple";
            const string cardNumber = "4256284623339010";
            const string cvv = "155";
            const string expiryDate = "07/27";
            Driver.Navigate().GoToUrl(AppServer.URL);
            Driver.Manage().Window.Size = new System.Drawing.Size(Display.DesktopWidth, Display.DesktopHeight);
            Driver.FindElement(By.Id("login")).Click();
            Driver.FindElement(By.Id("Input_Email")).SendKeys(TestClient.Email);
            Driver.FindElement(By.Id("Input_Password")).SendKeys(TestClient.Password);
            Driver.FindElement(By.Id("login-submit")).Click();
            Driver.FindElement(By.Id("accountDashboard")).Click();
            Driver.FindElement(By.Id("managePaymentInfo")).Click();
            Driver.FindElement(By.Id("Input_CardHolderName")).SendKeys(cardHolderName);
            Driver.FindElement(By.Id("Input_CardNumber")).SendKeys(cardNumber);
            Driver.FindElement(By.Id("Input_CVV")).SendKeys(cvv);
            Driver.FindElement(By.Id("Input_ExpiryDate")).SendKeys(expiryDate);
            TestUtils.ClickElementSafely("savePaymentInfoButton", Driver);
            Assert.Multiple(() =>
            {
                Assert.That(Driver.FindElement(By.Id("statusMessagePartialViewAlert")).Text, Is.EqualTo("Your Payment Info has been updated"));
                Driver.FindElement(By.Id("navLinkHome")).Click();
                TestUtils.ClickElementSafely("addToCartButton1", Driver);
                Driver.FindElement(By.Id("ViewCartButton")).Click();
                var preconfiguredCardHolderName = Driver.FindElement(By.Id("CardHolderName"))?.GetAttribute("value")?.ToString();
                var preconfiguredCardNumber = Driver.FindElement(By.Id("CardNumber"))?.GetAttribute("value")?.ToString();
                var preconfiguredCvv = Driver.FindElement(By.Id("CVV"))?.GetAttribute("value")?.ToString();
                var preconfiguredExpiryDate = Driver.FindElement(By.Id("ExpiryDate"))?.GetAttribute("value")?.ToString();
                Assert.That(preconfiguredCardHolderName, Is.EqualTo(cardHolderName));
                Assert.That(preconfiguredCardNumber, Is.EqualTo(cardNumber));
                Assert.That(preconfiguredCvv, Is.EqualTo(cvv));
                Assert.That(preconfiguredExpiryDate, Is.EqualTo(expiryDate));
            });
        }
    }
}
