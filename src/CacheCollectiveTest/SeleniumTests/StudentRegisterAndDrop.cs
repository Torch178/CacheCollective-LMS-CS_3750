using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using SeleniumExtras.WaitHelpers;

namespace CacheCollectiveTest.SeleniumTests
{
    [TestClass]
    public class StudentRegisterAndDrop
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [TestInitialize]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        [TestMethod]
        public void SeleniumTest()
        {
            driver.Navigate().GoToUrl("https://cachecollective.azurewebsites.net/Users/Login");

            Login();

            // Go to Registration Page
            driver.FindElement(By.LinkText("Registration")).Click();
            Thread.Sleep(1000);

            // Click on Register button for CS 4110
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            var registerLink = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//tr[td[contains(text(), 'Formal Language & Algorithms')]]//form/button[contains(text(), 'Register')]")));
            registerLink.Click();

            Thread.Sleep(1000);

            // Go to Home Page
            driver.FindElement(By.LinkText("Home")).Click();
            Thread.Sleep(1000);

            // Click on CS 4110 Details link
            var courseLink = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[.//h5[contains(text(), 'Formal Language & Algorithms')]]")));
            courseLink.Click();

            Thread.Sleep(3000);

            // Go to Registration Page
            driver.FindElement(By.LinkText("Registration")).Click();
            Thread.Sleep(1000);

            // Click on Drop button for CS 4110
            registerLink = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//tr[td[contains(text(), 'Formal Language & Algorithms')]]//form/button[contains(text(), 'Drop')]")));
            registerLink.Click();

            Thread.Sleep(1000);

            // Go to Home Page
            driver.FindElement(By.LinkText("Home")).Click();
            Thread.Sleep(1000);
        }

        private void Login()
        {
            driver.FindElement(By.Id("User_Email")).SendKeys("teststudent@test.com");
            driver.FindElement(By.Id("User_Password")).SendKeys("password");
            driver.FindElement(By.ClassName("btn")).Click();
        }

        [TestCleanup]
        public void Cleanup()
        {
            driver.Quit();
        }
    }
}
