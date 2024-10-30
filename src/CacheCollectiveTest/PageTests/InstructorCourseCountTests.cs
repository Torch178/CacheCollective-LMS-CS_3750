using Microsoft.VisualStudio.TestTools.UnitTesting;
using RazorPagesMovie.Models;
using RazorPagesMovie.Pages;
using RazorPagesMovie.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using RazorPagesMovie.Pages.Course;

namespace CacheCollectiveTest
{
    [TestClass]
    public class InstructorCourseCountTests
    {
        // Set up an in-memory database for isolated testing
        private RazorPagesMovieContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<RazorPagesMovieContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;

            return new RazorPagesMovieContext(options);
        }

        // Helper method to create a ClaimsPrincipal for an instructor user
        private ClaimsPrincipal CreateInstructorUser(int userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            return new ClaimsPrincipal(identity);
        }

        [TestMethod]
        public async Task OnPostAsync_ShouldIncrementInstructorCourseCount_WhenCourseCreated()
        {
            // Arrange
            var context = GetInMemoryContext();

            // Create an instructor user
            var instructorUser = new User
            {
                Id = 1,
                Email = "InstructorEmail@Test.com",
                Password = "TestPassword123",
                FirstName = "Instructor",
                LastName = "Smith",
                IsInstructor = true,
                City = "City",
                State = "State",
                StreetAddress = "1234 Example St",
                Zip = "12345"
            };
            context.User.Add(instructorUser);
            await context.SaveChangesAsync();

            // Initial course count for this instructor
            int initialCourseCount = await context.Course.CountAsync(c => c.InstructorId == instructorUser.Id);

            // Set up the page model with a new course for this instructor
            var pageModel = new CourseCreationModel(context)
            {
                CurrentCourse = new Course
                {
                    Department = Department.CS,
                    Number = 1020,
                    Title = "New Test Course",
                    Capacity = 10,
                    CreditHours = 3,
                    Location = "Campus",
                    InstructorId = instructorUser.Id
                },
                SelectedMeetingDays = new List<string> { "Tuesday", "Thursday" }
            };

            // Simulate a logged-in instructor
            pageModel.PageContext.HttpContext = new DefaultHttpContext
            {
                User = CreateInstructorUser(instructorUser.Id)
            };

            // Act
            var result = await pageModel.OnPostAsync();

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToPageResult)); // Ensure result is a redirect

            // Check if the course count has incremented by 1
            int updatedCourseCount = await context.Course.CountAsync(c => c.InstructorId == instructorUser.Id);
            Assert.AreEqual(initialCourseCount + 1, updatedCourseCount, "Course count should increment by 1.");
        }
    }
}
