using Microsoft.VisualStudio.TestTools.UnitTesting;
using RazorPagesMovie.Models;
using RazorPagesMovie.Pages;
using RazorPagesMovie.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesMovie.Pages.Course;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace CacheCollectiveTest
{
    [TestClass]
    public class CourseCreationModelTests
    {

        //This is to create an in memory database to use for this test only. Stops the test from being ruined by wrong live data
        private RazorPagesMovieContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<RazorPagesMovieContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;

            return new RazorPagesMovieContext(options);
        }

        //Creates a claim for an instructor user to use for the testmethods
        private ClaimsPrincipal CreateInstructorUser(int userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            return new ClaimsPrincipal(identity);
        }

        //Testmethod that is actually running the test - can create it for any methods in any pages
        [TestMethod]
        public async Task OnPostAsync_ShouldCreateCourse_WhenValidInstructor()
        {
            //Three steps in test methods
            // ---
            // Arrange
            // --- Set up the necessary objects, data, and conditions required for the test. This includes initializing variables, creating mock data, or setting up dependencies.
            var context = GetInMemoryContext();

            // Create valid instructor user
            var instructorUser = new User 
            { 
                Id = 1,
                Email = "TestEmail@Test.com",
                Password = "TestPassword123",
                FirstName = "Test",
                LastName = "McTestSon",
                IsInstructor = true,
                City = "Test",
                State= "UT",
                StreetAddress = "1234w 5678s",
                Zip = "12345"
            };
            context.User.Add(instructorUser);
            await context.SaveChangesAsync();

            // Set up page model now
            var pageModel = new CourseCreationModel(context)
            {
                CurrentCourse = new RazorPagesMovie.Models.Course 
                {
                    Department = Department.CS,
                    Number = 1010,
                    Title = "Test Course",
                    Capacity = 10,
                    CreditHours = 4,
                    Location = "Campus",
                },
                SelectedMeetingDays = new List<string> { "Monday", "Wednesday"}
            };

            // Simulate a logged in instructor
            pageModel.PageContext.HttpContext = new DefaultHttpContext
            {
                User = CreateInstructorUser(instructorUser.Id)
            };

            // ---
            // Act
            // --- Perform the action or method you want to test. This is where you call the method and execute the logic being tested.
            var result = await pageModel.OnPostAsync();


            // ---
            // Assert
            //  --- Verify that the outcome of the action is as expected. This involves checking results, validating state changes, or ensuring that the correct behavior occurred.
            Assert.IsInstanceOfType(result, typeof(RedirectToPageResult)); // Ensure the result is a redirect
            var course = await context.Course.FirstOrDefaultAsync(c => c.Title == "Test Course");
            Assert.IsNotNull(course); // Verify course was created
            Assert.AreEqual("Test McTestSon", course.Instructor); // Verify instructor name was set properly
            Assert.AreEqual("Monday, Wednesday", course.MeetingDays);

        }
    }
}