using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;
using RazorPagesMovie.Pages.Course.Assignment;
using RazorPagesMovie.Pages.Course.Assignment.Submissions;
using RazorPagesMovie.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CacheCollectiveTest.PageTests
{
    [TestClass]
    public class InstructorGradeTest
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

        //TempData For Testing
        private class TestTempDataProvider : ITempDataProvider
        {
            private readonly Dictionary<string, object> _tempData = new Dictionary<string, object>();

            public IDictionary<string, object> LoadTempData(HttpContext context) => _tempData;

            public void SaveTempData(HttpContext context, IDictionary<string, object> values)
            {
                foreach (var item in values)
                {
                    _tempData[item.Key] = item.Value;
                }
            }
        }

        [TestMethod]
        public async Task OnPostSubmitAsync_InstructorCanGradeCorrectly()
        {
            //Arrange
            var context = GetInMemoryContext();
            var notificationService = new NotificationService(context);

            var instructorUser = new RazorPagesMovie.Models.User
            {
                Id = 4,
                Email = "TestEmail@Test.com",
                Password = "TestPassword123",
                FirstName = "Test",
                LastName = "McTestSon",
                IsInstructor = true,
                City = "Test",
                State = "UT",
                StreetAddress = "1234w 5678s",
                Zip = "12345"
            };
            context.User.Add(instructorUser);
            await context.SaveChangesAsync();

            var testCourse = new Course
            {
                CourseId = 2,
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
                Id = 2,
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
                SubmissionId = 3,
                AssignmentId = testAssignment.Id,
                UserId = instructorUser.Id,
                SubmissionType = SubmissionType.TextEntry,
                SubmissionDate = DateTime.Now
            };
            context.Submission.Add(testSubmission);
            await context.SaveChangesAsync();

            var tempDataProvider = new TestTempDataProvider();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), tempDataProvider);

            var pageModel = new GradingModel(context, notificationService)
            {
                TempData = tempData,
                Submission = testSubmission
            };

            pageModel.PageContext.HttpContext = new DefaultHttpContext
            {
                User = CreateUser(instructorUser.Id),
            };
            pageModel.PageContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "GradedPoints", "7" },
                { "InstructorComments", "Good job!" }
            });

            //Act
            var result = await pageModel.OnPostAsync(3);

            //Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToPageResult));
            // Check that the submission was updated
            var submission = await context.Submission.FirstOrDefaultAsync(s => s.UserId == testSubmission.UserId && s.AssignmentId == testSubmission.AssignmentId);
            Assert.IsNotNull(submission); // Ensure submission exists
            Assert.AreEqual(7, submission.GradedPoints);
            Assert.AreEqual("Good job!", submission.InstructorComments);
        }
    }
}
