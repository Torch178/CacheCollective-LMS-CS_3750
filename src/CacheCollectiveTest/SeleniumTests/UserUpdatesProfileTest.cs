using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace CacheCollectiveTest.SeleniumTests
{
    [TestClass]
    public class UserUpdatesProfile
    {
        private IWebDriver driver;

        [TestInitialize]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }

        [TestMethod]
        public void TestProfileUpdates()
        {
            driver.Navigate().GoToUrl("https://cachecollective.azurewebsites.net/Users/Login");
            Thread.Sleep(1000);

            var emailInput = driver.FindElement(By.Id("User_Email"));
            emailInput.SendKeys("hankbank123@example.com");
            Thread.Sleep(1000);

            var passInput = driver.FindElement(By.Id("User_Password"));
            passInput.SendKeys("gg2easy43");
            Thread.Sleep(1000);

            var submitButton = driver.FindElement(By.ClassName("btn"));
            submitButton.Click();
            Thread.Sleep(1000);

            Assert.IsTrue(driver.Title.Contains("Home"));

            //navigate to profile
            driver.Navigate().GoToUrl("https://cachecollective.azurewebsites.net/Users/Profile");
            Thread.Sleep(1000);
            var editLink = driver.FindElement(By.Id("edit_profile_link"));

            //click on edit profile link
            editLink.Click();
            Thread.Sleep(1000);
            Assert.IsTrue(driver.Title.Contains("Edit Profile"));

            //enter new information into edit profile form
            submitButton = driver.FindElement(By.ClassName("btn"));
            var passInput1 = driver.FindElement(By.Id("fname"));
            var passInput2 = driver.FindElement(By.Id("lname"));
            var passInput3 = driver.FindElement(By.Id("apartmentNum"));
            var passInput4 = driver.FindElement(By.Id("city"));
            passInput1.Clear();
            passInput2.Clear();
            passInput3.Clear();
            passInput4.Clear();
            Thread.Sleep(1000);
            passInput1.SendKeys("Michael");
            passInput2.SendKeys("Reeves");
            passInput3.SendKeys("2B");
            passInput4.SendKeys("Logan");
            Thread.Sleep(1000);

            //submit form
            submitButton.Click();
            Thread.Sleep(1000);

            //Assert that the profile page reloads with new information in relevant fields
            Assert.IsTrue(driver.Title.Equals("Profile"));
            var fname = driver.FindElement(By.Id("fname"));
            var lname = driver.FindElement(By.Id("lname"));
            var apartmentNum = driver.FindElement(By.Id("apartmentNum"));
            var city = driver.FindElement(By.Id("city"));
            Assert.IsTrue(fname.Text.Contains("Michael"));
            Assert.IsTrue(lname.Text.Contains("Reeves"));
            Assert.IsTrue(apartmentNum.Text.Contains("2B"));
            Assert.IsTrue(city.Text.Contains("Logan"));

        }

        [TestCleanup]
        public void Cleanup()
        {
            driver.Quit();
        }
    }
}
