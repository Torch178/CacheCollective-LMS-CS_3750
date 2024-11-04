using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;
using RazorPagesMovie.Pages.Course.Assignment;
using System.Security.Claims;

namespace CacheCollectiveTest.PageTests
{
    [TestClass]
    public class StudentSubmitTest
    {
        //This is to create an in memory database to use for this test only. Stops the test from being ruined by wrong live data
        private RazorPagesMovieContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<RazorPagesMovieContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .EnableSensitiveDataLogging()
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
        public async Task OnPostSubmitAsync_StudentShouldSubmitCorrectly()
        {
            //Arrange
            var context = GetInMemoryContext();

            var studentUser = new RazorPagesMovie.Models.User
            {
                Id = 2,
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

            var testCourse = new Course
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

            var testAssignment = new Assignment
            {
                Id = 1,
                CourseId = 1,
                Title = "Test Assignment",
                Description = "Test",
                MaxPoints = 10,
                DueDate = DateTime.Now + TimeSpan.FromDays(1),
                SubmissionType = SubmissionType.TextEntry
            };
            context.Assignment.Add(testAssignment);
            await context.SaveChangesAsync();

            var testSubmission = new Submission
            {
                AssignmentId = 1,
                UserId = 1
            };

            var pageModel = new SubmitModel(context)
            {
                CurrentUser = studentUser,
                AssignmentId = 1,
                CourseId = 1,
                SubmittedText = "Test",
                Submission = testSubmission
            };

            pageModel.PageContext.HttpContext = new DefaultHttpContext
            {
                User = CreateUser(studentUser.Id)
            };

            //Act
            var result = await pageModel.OnPostAsync();

            //Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToPageResult));
            // Check that the submission was created
            var submission = await context.Submission.FirstOrDefaultAsync(s => s.UserId == studentUser.Id && s.AssignmentId == testAssignment.Id);
            Assert.IsNotNull(submission); // Ensure submission exists
        }
    }
}
