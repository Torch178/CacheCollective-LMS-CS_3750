using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Threading;

namespace CacheCollectiveTest.SeleniumTests
{
    [TestClass]
    public class GradingSubmissionTest
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
        public void TestInstructorGradesSubmission()
        {
            // Log in as an instructor
            driver.Navigate().GoToUrl("https://cachecollective.azurewebsites.net/Users/Login");
            Login();

            // Navigate to the submission grading page
            driver.Navigate().GoToUrl("https://cachecollective.azurewebsites.net/Course/Assignment/Submissions/Grading?submissionId=6");

            Thread.Sleep(1000); // Wait to make sure the page is loaded

            // Enter the grade
            var gradeInput = driver.FindElement(By.Id("GradedPoints"));
            gradeInput.Clear();  // Clear any existing value
            gradeInput.SendKeys("10");

            Thread.Sleep(1000);  // Optional wait

            // Enter a comment (e.g., "Great work on the assignment!")
            var commentInput = driver.FindElement(By.Id("InstructorComments"));
            commentInput.Clear();  // Clear any existing comment
            commentInput.SendKeys("Great work on the assignment!");

            Thread.Sleep(1000);  // Optional wait

            // Click the "Save" button using JavaScript (to ensure it's clicked)
            var saveButton = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector("button[type='submit']")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", saveButton);

            // Wait for success message to appear (ensure the grade was saved)
            var successMessage = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(".alert.alert-success")));
            Assert.IsTrue(successMessage.Text.Contains("Grade saved successfully!"));

            Thread.Sleep(1000);  // Optional wait

            // Check if you are redirected to the submissions page
            Assert.IsTrue(driver.Url.Contains("Submissions"));
        }

        private void Login()
        {
            driver.FindElement(By.Id("User_Email")).SendKeys("batusens@gmail.com");
            driver.FindElement(By.Id("User_Password")).SendKeys("pass2");
            driver.FindElement(By.ClassName("btn")).Click();
        }

        [TestCleanup]
        public void Cleanup()
        {
            driver.Quit();
        }
    }
}