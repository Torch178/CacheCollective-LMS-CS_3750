using Microsoft.VisualStudio.TestTools.UnitTesting;
using RazorPagesMovie.Models;
using RazorPagesMovie.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesMovie.Pages.Users;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CacheCollectiveTest
{
    [TestClass]
    public class StudentRegistrationTest
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
        private ClaimsPrincipal CreateUser(int userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            return new ClaimsPrincipal(identity);
        }

        [TestMethod]
        public async Task OnPostRegistrationAsync_StudentShouldRegisterCorrectly()
        {
            //Arrange
            var context = GetInMemoryContext();

            var studentUser = new RazorPagesMovie.Models.User
            {
                Id = 1,
                Email = "TestEmail@Test.com",
                Password = "TestPassword123",
                FirstName = "Test",
                LastName = "McTestSon",
                IsInstructor = false,
                City = "Test",
                State = "UT",
                StreetAddress = "1234w 5678s",
                Zip = "12345"
            };
            context.User.Add(studentUser);
            await context.SaveChangesAsync();

            var testCourse = new RazorPagesMovie.Models.Course
            {
                CourseId = 1,
                Department = Department.CS,
                Number = 1010,
                Title = "Test Course",
                Capacity = 10,
                CreditHours = 4,
                Location = "Campus",
                MeetingDays = "Monday, Wednesday"
            };
            context.Course.Add(testCourse);
            await context.SaveChangesAsync();

            var pageModel = new RegistrationModel(context)
            {
                CurrentUser = studentUser,
                Course = new List<RazorPagesMovie.Models.Course> { testCourse }
            };

            pageModel.PageContext.HttpContext = new DefaultHttpContext
            {
                User = CreateUser(studentUser.Id)
            };


            //Act
            var result = await pageModel.OnPostRegistrationAsync(testCourse.CourseId);


            //Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToPageResult));
            // Check that the enrollment was created
            var enrollment = await context.Enrollment.FirstOrDefaultAsync(e => e.UserId == studentUser.Id && e.CourseId == testCourse.CourseId);
            Assert.IsNotNull(enrollment); // Ensure enrollment exists
            await context.Database.EnsureDeletedAsync();
        }

    }
}