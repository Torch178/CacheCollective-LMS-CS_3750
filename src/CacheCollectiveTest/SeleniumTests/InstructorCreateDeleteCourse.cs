using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace CacheCollectiveTest.SeleniumTests
{
    [TestClass]
    public class InstructorCreateDeleteCourse
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
        public void TestFirstSelenium()
        {
            driver.Navigate().GoToUrl("https://cachecollective.azurewebsites.net/Users/Login");

            Login();

            // Go to Course page
            var coursesLink = wait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText("Courses")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", coursesLink);

            var createNewButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText("Create New")));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", createNewButton);

            // Make a dictionary for filling the form
            var formData = new Dictionary<string, string>
            {
                { "CurrentCourse.Department", "MUS" },
                { "CurrentCourse.Number", "1200" },
                { "CurrentCourse.Title", "Music Theory 3" },
                { "CurrentCourse.Capacity", "10" },
                { "CurrentCourse.CreditHours", "2" },
                { "CurrentCourse.MeetingDays", "Monday,Tuesday,Wednesday,Thursday" },
                { "CurrentCourse.StartTime", "09:30AM" },  // Standard format
                { "CurrentCourse.EndTime", "10:20AM" },    // Standard format
                { "CurrentCourse.Location", "Room 204" }
            };

            FillForm("form", formData);
            Thread.Sleep(1000);

            // Submit
            var submitButton = driver.FindElement(By.CssSelector("button[type='submit']"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", submitButton);
            Thread.Sleep(1000);

            foreach (var entry in formData)
            {
                if (entry.Key == "CurrentCourse.MeetingDays")
                {
                    foreach (var day in entry.Value.Split(','))
                    {
                        Assert.IsTrue(driver.PageSource.Contains(day.Trim()));
                    }
                }
                else if (entry.Key.Contains("Time"))
                {
                    // Extract hour and minute (e.g., "09:30") from the entry time
                    string formattedTime = DateTime.ParseExact(entry.Value, "hh:mmtt", null).ToString("hh:mm");

                    // Assert that the page contains the time in "hh:mm" format
                    Assert.IsTrue(driver.PageSource.Contains(formattedTime), $"Time '{formattedTime}' was not found on the page.");
                }
                else
                {
                    Assert.IsTrue(driver.PageSource.Contains(entry.Value));
                }
            }

            // Locate and click the "Delete" link for the specific course row
            var deleteLink = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//tr[td[contains(text(), 'Music Theory 3')]]//a[contains(text(), 'Delete')]")));
            deleteLink.Click();

            var confirmDeleteButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[type='submit']")));
            confirmDeleteButton.Click();


        }

        private void Login()
        {
            driver.FindElement(By.Id("User_Email")).SendKeys("testinstructor@test.com");
            driver.FindElement(By.Id("User_Password")).SendKeys("Password");
            driver.FindElement(By.ClassName("btn")).Click();
        }

        private void FillForm(string formSelector, Dictionary<string, string> formData)
        {
            var form = driver.FindElement(By.CssSelector(formSelector));

            foreach (var entry in formData)
            {
                if (entry.Key == "CurrentCourse.MeetingDays")
                {
                    foreach (var day in entry.Value.Split(','))
                    {
                        var checkbox = form.FindElement(By.XPath($"//input[@name='SelectedMeetingDays'][@value='{day.Trim()}']"));
                        if (!checkbox.Selected)
                        {
                            checkbox.Click();
                        }
                    }
                }
                else
                {
                    var element = form.FindElement(By.Name(entry.Key));

                    if (element.TagName == "select")
                    {
                        var selectElement = new SelectElement(element);
                        selectElement.SelectByValue(entry.Value);
                    }
                    else
                    {
                        element.Clear();
                        element.SendKeys(entry.Value);
                    }
                }
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            driver.Quit();
        }
    }
}
