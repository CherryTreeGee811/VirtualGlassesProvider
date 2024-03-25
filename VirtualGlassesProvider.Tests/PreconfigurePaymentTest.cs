using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


namespace VirtualGlassesProvider.Tests
{
    [TestFixture, Order(6)]
    internal class PreconfigurePaymentTest
    {
        #pragma warning disable NUnit1032
        private ChromeDriver _driver { get; set; }
        #pragma warning restore NUnit1032


        [SetUp]
        public void SetUp()
        {
            ChromeOptions options = new ChromeOptions { AcceptInsecureCertificates = true };
            options.AddArgument("--headless=new");
            options.AddUserProfilePreference("autofill.credit_card_enabled", false);
            _driver = new ChromeDriver(options);
        }


        [TearDown]
        protected void TearDown()
        {
            _driver.Quit();
        }


        [Test, Order(1)]
        public void ClientDoesNotPreconfigurePayment()
        {
            _driver.Navigate().GoToUrl(AppServer.URL);
            _driver.Manage().Window.Size = new System.Drawing.Size(1936, 1056);
            _driver.FindElement(By.Id("login")).Click();
            _driver.FindElement(By.Id("Input_Email")).Click();
            _driver.FindElement(By.Id("Input_Email")).SendKeys(TestClient.Email);
            _driver.FindElement(By.Id("Input_Password")).Click();
            _driver.FindElement(By.Id("Input_Password")).SendKeys(TestClient.Password);
            _driver.FindElement(By.Id("login-submit")).Click();
            _driver.FindElement(By.CssSelector(".col-md-4:nth-child(1) .btn-success")).Click();
            _driver.FindElement(By.CssSelector(".fa")).Click();
            var preconfiguredCardHolderName = _driver.FindElement(By.Id("CardHolderName")).GetAttribute("value").ToString();
            var preconfiguredCardNumber = _driver.FindElement(By.Id("CardNumber")).GetAttribute("value").ToString();
            var preconfiguredCvv = _driver.FindElement(By.Id("CVV")).GetAttribute("value").ToString();
            var preconfiguredExpiryDate = _driver.FindElement(By.Id("ExpiryDate")).GetAttribute("value").ToString();
            Assert.That(preconfiguredCardHolderName, Is.EqualTo(""));
            Assert.That(preconfiguredCardNumber, Is.EqualTo(""));
            Assert.That(preconfiguredCvv, Is.EqualTo(""));
            Assert.That(preconfiguredExpiryDate, Is.EqualTo(""));
        }


        [Test, Order(2)]
        public void ClientPreconfiguresPayment()
        {
            const string cardHolderName = "Tim Apple";
            const string cardNumber = "4256284623339010";
            const string cvv = "155";
            const string expiryDate = "07/27";
            _driver.Navigate().GoToUrl(AppServer.URL);
            _driver.Manage().Window.Size = new System.Drawing.Size(1936, 1056);
            _driver.FindElement(By.Id("login")).Click();
            _driver.FindElement(By.Id("Input_Email")).Click();
            _driver.FindElement(By.Id("Input_Email")).SendKeys(TestClient.Email);
            _driver.FindElement(By.Id("Input_Password")).Click();
            _driver.FindElement(By.Id("Input_Password")).SendKeys(TestClient.Password);
            _driver.FindElement(By.Id("login-submit")).Click();
            _driver.FindElement(By.Id("manage")).Click();
            _driver.FindElement(By.Id("paymentInfo")).Click();
            _driver.FindElement(By.Id("Input_CardHolderName")).SendKeys(cardHolderName);
            _driver.FindElement(By.Id("Input_CardNumber")).SendKeys(cardNumber);
            _driver.FindElement(By.Id("Input_CVV")).SendKeys(cvv);
            _driver.FindElement(By.Id("Input_ExpiryDate")).SendKeys(expiryDate);
            _driver.FindElement(By.ClassName("btn-primary")).Click();
            Assert.That(_driver.FindElement(By.ClassName("alert")).Text, Is.EqualTo("Your Payment Info has been updated"));
            _driver.FindElement(By.CssSelector("body > header > nav > div > div > ul > li:nth-child(1) > a")).Click();
            _driver.FindElement(By.CssSelector("body > div > main > div:nth-child(5) > div:nth-child(1) > div > div.card-body > a.btn.btn-success")).Click();
            _driver.FindElement(By.CssSelector(".fa")).Click();
            var preconfiguredCardHolderName = _driver.FindElement(By.Id("CardHolderName")).GetAttribute("value").ToString();
            var preconfiguredCardNumber = _driver.FindElement(By.Id("CardNumber")).GetAttribute("value").ToString();
            var preconfiguredCvv = _driver.FindElement(By.Id("CVV")).GetAttribute("value").ToString();
            var preconfiguredExpiryDate = _driver.FindElement(By.Id("ExpiryDate")).GetAttribute("value").ToString();
            Assert.That(preconfiguredCardHolderName, Is.EqualTo(cardHolderName));
            Assert.That(preconfiguredCardNumber, Is.EqualTo(cardNumber));
            Assert.That(preconfiguredCvv, Is.EqualTo(cvv));
            Assert.That(preconfiguredExpiryDate, Is.EqualTo(expiryDate));
        }
    }
}
