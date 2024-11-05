using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;


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
            driver.FindElement(By.LinkText("Courses")).Click();
            driver.FindElement(By.LinkText("Create New")).Click();

            // Make a dictionary for filling the form
            var formData = new Dictionary<string, string>
            {
                { "CurrentCourse.Department", "MUS" },
                { "CurrentCourse.Number", "1200" },
                { "CurrentCourse.Title", "Music Theory 3" },
                { "CurrentCourse.Capacity", "10" },
                { "CurrentCourse.CreditHours", "2" },
                { "CurrentCourse.MeetingDays", "Monday,Tuesday,Wednesday,Thursday" },
                { "CurrentCourse.StartTime", "930AM" },
                { "CurrentCourse.EndTime", "1020AM" },
                { "CurrentCourse.Location", "Room 204" }
            };

            FillForm("form", formData);
            Thread.Sleep(1000);

            // Submit
            driver.FindElement(By.CssSelector("button[type='submit']")).Click();
            Thread.Sleep(1000);

            foreach (var entry in formData)
            {
                if (entry.Key == "CurrentCourse.MeetingDays")
                {
                    foreach (var day in entry.Value.Split(","))
                    {
                        Assert.IsTrue(driver.PageSource.Contains(day.Trim()));
                    }
                }
                else if (entry.Key.Contains("Time"))
                {
                    string formattedTime = DateTime.Parse(entry.Value).ToString("HH:mm:ss");
                    Assert.IsTrue(driver.PageSource.Contains(formattedTime));
                }
                else
                {
                    Assert.IsTrue(driver.PageSource.Contains(entry.Value));
                }
            }

        }

        private void Login()
        {
            driver.FindElement(By.Id("User_Email")).SendKeys("Camerontrejo2000@gmail.com");
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
