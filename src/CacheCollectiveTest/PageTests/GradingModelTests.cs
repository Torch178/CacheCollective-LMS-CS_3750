using Microsoft.VisualStudio.TestTools.UnitTesting;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using Azure;
using Humanizer;

namespace RazorPagesMovie.Pages.Course.Assignment.Submissions.Tests
{
    [TestClass]
    public class GradingModelTests
    {
        private RazorPagesMovieContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<RazorPagesMovieContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;

            return new RazorPagesMovieContext(options);
        }

        private ClaimsPrincipal CreateUser(int userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            return new ClaimsPrincipal(identity);
        }

        // Verifies that the method returns a NotFoundResult when the submission ID is null
        [TestMethod]
        public async Task OnGetAsync_ShouldReturnNotFound_WhenSubmissionIdIsNull()
        {
            // Arrange
            var context = GetInMemoryContext();
            //ensure database is cleared and in a clean state (empty) for testing
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
            var pageModel = new GradingModel(context);

            // Act
            var result = await pageModel.OnGetAsync(null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        // Checks that when invalid graded points(non-numeric input) are submitted,
        // the method returns the same page instead of redirecting
        [TestMethod]
        public async Task OnPostAsync_ShouldReturnPage_WhenGradedPointsAreInvalid()
        {
            // Arrange
            var context = GetInMemoryContext();
            //ensure database is cleared and in a clean state (empty) for testing
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
            var submission = new Submission
            {
                SubmissionId = 1,
                AssignmentId = 1
            };
            context.Submission.Add(submission);
            await context.SaveChangesAsync();

            var pageModel = new GradingModel(context);
            pageModel.Submission = submission;

            // Simulate the user and invalid form data
            pageModel.PageContext.HttpContext = new DefaultHttpContext
            {
                User = CreateUser(1), // Simulate an instructor user
                Request =
                {
                    Form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
                    {
                        { "GradedPoints", "InvalidPoints" },
                        { "InstructorComments", "Needs improvement." }
                    })
                }
            };

            // Act
            var result = await pageModel.OnPostAsync(submission.SubmissionId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(PageResult));
            Assert.IsTrue(pageModel.ModelState.ContainsKey("GradedPoints"));
            Assert.IsFalse(pageModel.ModelState.IsValid); // Ensure the model state is invalid
        }
    }
}
