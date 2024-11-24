using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Threading;

namespace RazorPagesMovie.Tests
{
    [TestClass]
    public class CreateAccountTests
    {
        private IWebDriver driver;

        [TestInitialize]
        public void Setup()
        {
            // Initialize the Chrome WebDriver
            driver = new ChromeDriver();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }

        [TestMethod]
        public void TestCreateUserAccount()
        {
            // Step 1: Navigate to the Create Account page
            driver.Navigate().GoToUrl("https://cachecollective.azurewebsites.net/Users/Create");
            Thread.Sleep(1000);

            // Step 2: Locate the form fields and fill them out

            var firstNameInput = driver.FindElement(By.Id("User_FirstName"));
            firstNameInput.SendKeys("John");
            Thread.Sleep(500);

            var lastNameInput = driver.FindElement(By.Id("User_LastName"));
            lastNameInput.SendKeys("Doe");
            Thread.Sleep(500);

            var birthdateInput = driver.FindElement(By.Id("User_Birthdate"));
            birthdateInput.SendKeys("01/01/1990");
            Thread.Sleep(500);

            var emailInput = driver.FindElement(By.Id("User_Email"));
            emailInput.SendKeys("john.doe@example.com");
            Thread.Sleep(500);

            var passwordInput = driver.FindElement(By.Id("User_Password"));
            passwordInput.SendKeys("Password123!");
            Thread.Sleep(500);

            // Select "Yes" for IsInstructor
            var instructorRadioButton = driver.FindElement(By.Id("yes"));
            instructorRadioButton.Click();
            Thread.Sleep(500);

            var streetAddressInput = driver.FindElement(By.Id("User_StreetAddress"));
            streetAddressInput.SendKeys("123 Main St");
            Thread.Sleep(500);

            var apartmentInput = driver.FindElement(By.Id("User_ApartmentNum"));
            apartmentInput.SendKeys("101");
            Thread.Sleep(500);

            var cityInput = driver.FindElement(By.Id("User_City"));
            cityInput.SendKeys("Metropolis");
            Thread.Sleep(500);

            var stateDropdown = driver.FindElement(By.Id("User_State"));
            var selectState = new OpenQA.Selenium.Support.UI.SelectElement(stateDropdown);
            selectState.SelectByText("California");
            Thread.Sleep(500);

            var zipInput = driver.FindElement(By.Id("User_Zip"));
            zipInput.SendKeys("90001");
            Thread.Sleep(500);

            // Step 3: Locate the submit button and click it
            var submitButton = driver.FindElement(By.CssSelector("input[type='submit']"));
            submitButton.Click();
            Thread.Sleep(2000);  // Wait for page redirect

            Assert.IsTrue(driver.Title.Contains("Home"));
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Close the browser and clean up after the test
            driver.Quit();
        }
    }
}
