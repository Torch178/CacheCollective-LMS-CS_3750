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
using System.Linq;
using System.Threading.Tasks;

namespace CacheCollectiveTest
{
    [TestClass]
    public class CourseEditingModelTests
    {
        //This is to create an in-memory database to use for this test only. Stops the test from being ruined by wrong live data
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

        [TestMethod]
        public async Task OnPostAsync_ShouldEditCourse_WhenValidInstructor()
        {
            // --- Arrange ---
            // Set up the necessary objects, data, and conditions required for the test.
            var context = GetInMemoryContext();
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            // Create valid instructor user
            var instructorUser = new User
            {
                Id = 1,
                Email = "instructor@test.com",
                Password = "TestPassword123",
                FirstName = "John",
                LastName = "Doe",
                IsInstructor = true,
                City = "Test",
                State = "UT",
                StreetAddress = "1234 Main St",
                Zip = "12345"
            };
            context.User.Add(instructorUser);
            await context.SaveChangesAsync();

            // Create a sample course that will be edited later
            var course = new RazorPagesMovie.Models.Course
            {
                CourseId = 1,
                Department = Department.CS,
                Number = 1010,
                Title = "Original Course",
                Capacity = 30,
                CreditHours = 3,
                Location = "Room 101",
                MeetingDays = "Monday, Wednesday",
                StartTime = DateTime.Parse("10:00 AM").TimeOfDay, // Convert DateTime to TimeSpan
                EndTime = DateTime.Parse("11:30 AM").TimeOfDay,  // Convert DateTime to TimeSpan
                Instructor = "John Doe",
                InstructorCourseId = instructorUser.Id
            };
            context.Course.Add(course);
            await context.SaveChangesAsync();

            // Set up the page model for editing the course
            var pageModel = new EditModel(context)
            {
                CurrentCourse = course,
                SelectedMeetingDays = new List<string> { "Monday", "Thursday" }
            };

            // Simulate a logged-in instructor
            pageModel.PageContext.HttpContext = new DefaultHttpContext
            {
                User = CreateInstructorUser(instructorUser.Id)
            };

            // --- Act ---
            // Perform the action (edit the course).
            pageModel.CurrentCourse.Title = "Updated Course Title";
            pageModel.CurrentCourse.Capacity = 25;
            var result = await pageModel.OnPostAsync();

            // --- Assert ---
            // Verify that the result is a redirect (indicating the course was saved and the page was redirected).
            Assert.IsInstanceOfType(result, typeof(RedirectToPageResult)); // Ensure the result is a redirect

            // Fetch the updated course from the database
            var updatedCourse = await context.Course.FirstOrDefaultAsync(c => c.CourseId == course.CourseId);

            // Verify that the course was updated correctly
            Assert.IsNotNull(updatedCourse);
            Assert.AreEqual("Updated Course Title", updatedCourse.Title); // Verify title was updated
            Assert.AreEqual(25, updatedCourse.Capacity); // Verify capacity was updated
            Assert.AreEqual("Monday, Thursday", updatedCourse.MeetingDays); // Verify meeting days were updated
            Assert.AreEqual("John Doe", updatedCourse.Instructor); // Verify instructor name stayed the same
        }

        [TestMethod]
        public async Task OnPostAsync_ShouldFail_WhenNoMeetingDaysSelected()
        {
            // --- Arrange ---
            var context = GetInMemoryContext();
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            // Create valid instructor user
            var instructorUser = new User
            {
                Id = 1,
                Email = "instructor@test.com",
                Password = "TestPassword123",
                FirstName = "John",
                LastName = "Doe",
                IsInstructor = true,
                City = "Test",
                State = "UT",
                StreetAddress = "1234 Main St",
                Zip = "12345"
            };
            context.User.Add(instructorUser);
            await context.SaveChangesAsync();

            // Create a sample course that will be edited later
            var course = new RazorPagesMovie.Models.Course
            {
                CourseId = 1,
                Department = Department.CS,
                Number = 1010,
                Title = "Original Course",
                Capacity = 30,
                CreditHours = 3,
                Location = "Room 101",
                MeetingDays = "Monday, Wednesday",
                StartTime = DateTime.Parse("10:00 AM").TimeOfDay, // Convert DateTime to TimeSpan
                EndTime = DateTime.Parse("11:30 AM").TimeOfDay,  // Convert DateTime to TimeSpan
                Instructor = "John Doe",
                InstructorCourseId = instructorUser.Id
            };
            context.Course.Add(course);
            await context.SaveChangesAsync();

            // Set up the page model for editing the course
            var pageModel = new EditModel(context)
            {
                CurrentCourse = course,
                SelectedMeetingDays = new List<string>() // No meeting days selected
            };

            // Simulate a logged-in instructor
            pageModel.PageContext.HttpContext = new DefaultHttpContext
            {
                User = CreateInstructorUser(instructorUser.Id)
            };

            // --- Act ---
            // Attempt to submit the form with no meeting days selected
            var result = await pageModel.OnPostAsync();

            // --- Assert ---
            // Verify that the result is still the Edit page (since validation failed)
            Assert.IsInstanceOfType(result, typeof(PageResult));

            // Ensure that the validation error for selected meeting days is present
            Assert.IsTrue(pageModel.ModelState.ContainsKey("SelectedMeetingDays"));
            Assert.AreEqual("Please select at least one meeting day.", pageModel.ModelState["SelectedMeetingDays"].Errors[0].ErrorMessage);
        }
    }
}
