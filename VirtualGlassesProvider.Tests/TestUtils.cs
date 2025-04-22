using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;


namespace VirtualGlassesProvider.Tests
{
    internal static class TestUtils
    {
        internal static void ClickElementSafely(ref IWebElement element, IWebDriver driver)
        {
            var wait = new WebDriverWait(driver, new TimeSpan(0, 0, 0, 10));
            new Actions(driver)
            .ScrollToElement(element)
            .Perform();
            var elementVisible = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(element));
            elementVisible.Click();
        }
    }
}
