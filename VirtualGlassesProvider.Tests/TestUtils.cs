using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;


namespace VirtualGlassesProvider.Tests
{
    internal static class TestUtils
    {
        internal static void ClickElementSafely(string elementId, IWebDriver driver)
        {
            try
            {
                // Use JavaScript to retrieve and click the element
                var script = @"
                    var element = document.getElementById(arguments[0]);
                    if (!element) {
                        throw new Error('Element with ID ' + arguments[0] + ' was not found.');
                    }
                    element.scrollIntoView({ block: 'center' });
                    element.click();
                ";
                ((IJavaScriptExecutor)driver).ExecuteScript(script, elementId);
            }
            catch (Exception ex)
            {
                // Log any other exceptions
                Console.WriteLine($"Error clicking element: {ex.Message}");
                throw;
            }
        }


        internal static IWebElement WaitForElementToBeVisible(By locator, IWebDriver driver, int timeoutInSeconds = 10)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
            return wait.Until(ExpectedConditions.ElementIsVisible(locator));
        }
    }
}
