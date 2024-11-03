using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace CacheCollectiveTest.SeleniumTests
{
    [TestClass]
    public class FirstSeleniumTest
    {
        private IWebDriver driver;

        [TestInitialize]
        public void Setup() 
        {
            driver = new ChromeDriver();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }

        [TestMethod]
        public void TestFirstSelenium()
        {
            driver.Navigate().GoToUrl("https://cachecollective.azurewebsites.net/Users/Login");
            Thread.Sleep(1000);

            var emailInput = driver.FindElement(By.Id("User_Email"));
            emailInput.SendKeys("Camerontrejo2000@gmail.com");
            Thread.Sleep(1000);

            var passInput = driver.FindElement(By.Id("User_Password"));
            passInput.SendKeys("Password");
            Thread.Sleep(1000);

            var submitButton = driver.FindElement(By.ClassName("btn"));
            submitButton.Click();
            Thread.Sleep(1000);

            Assert.IsTrue(driver.Title.Contains("Home"));
        }

        [TestCleanup]
        public void Cleanup()
        {
            driver.Quit();
        }
    }
}
