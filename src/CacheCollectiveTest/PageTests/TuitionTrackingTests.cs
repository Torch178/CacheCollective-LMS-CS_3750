using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;
using RazorPagesMovie.Pages.Course;
using RazorPagesMovie.Pages.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CacheCollectiveTest.PageTests
{
    [TestClass()]
    public class TuitionTrackingTests
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
        private ClaimsPrincipal CreateStudent(int userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            return new ClaimsPrincipal(identity);
        }

        [TestMethod()]
        public async Task TuitionTrackingTest()
        {
            //Three steps in test methods
            // ---
            // Arrange
            // --- Set up the necessary objects, data, and conditions required for the test. This includes initializing variables, creating mock data, or setting up dependencies.
            var context = GetInMemoryContext();

            // Create valid student user
            var student = new User
            {
                Id = 1,
                Email = "TestEmail@Test.com",
                Password = "TestPassword123",
                FirstName = "Test",
                LastName = "McTestSon",
                Birthdate = DateTime.MinValue,
                IsInstructor = false,
                City = "Test",
                State = "UT",
                StreetAddress = "1234w 5678s",
                Zip = "12345",
                tuitionDue = 0,
                tuitionPaid = 0,
                refundAmt = 0,
            };
            //create courses for enrollment
            var course1 = new Course
            {
                CourseId = 1,
                InstructorCourseId = 1,
                Department = Department.CS,
                Number = 1010,
                Title = "Test Course",
                Capacity = 10,
                CreditHours = 4,
                MeetingDays = "Monday, Friday",
                StartTime = TimeSpan.Zero,
                EndTime = TimeSpan.Zero,
                Location = "Campus",

            };
            var course2 = new Course
            {
                CourseId = 2,
                InstructorCourseId = 1,
                Department = Department.CS,
                Number = 2010,
                Title = "Test Course",
                Capacity = 10,
                CreditHours = 4,
                MeetingDays = "Monday, Friday",
                StartTime = TimeSpan.Zero,
                EndTime = TimeSpan.Zero,
                Location = "Campus",

            };
            var course3 = new Course
            {
                CourseId = 3,
                InstructorCourseId = 1,
                Department = Department.CS,
                Number = 3010,
                Title = "Test Course",
                Capacity = 10,
                CreditHours = 4,
                MeetingDays = "Monday, Friday",
                StartTime = TimeSpan.Zero,
                EndTime = TimeSpan.Zero,
                Location = "Campus",

            };
            context.User.Add(student);
            await context.SaveChangesAsync();
            context.Course.Add(course1);
            await context.SaveChangesAsync();
            context.Course.Add(course2);
            await context.SaveChangesAsync();
            context.Course.Add(course3);
            await context.SaveChangesAsync();

            // Set up page model now
            var regPageModel = new RegistrationModel(context)
            {
                CurrentUser = student,
                Course = new List<Course> { course1, course2, course3 },
            };

            // Simulate a logged in student
            regPageModel.PageContext.HttpContext = new DefaultHttpContext
            {
                User = CreateStudent(student.Id)
            };

            // ---
            // Act
            // --- Perform the action or method you want to test. This is where you call the method and execute the logic being tested.
            // ---
            // Assert
            //  --- Verify that the outcome of the action is as expected. This involves checking results, validating state changes, or ensuring that the correct behavior occurred.
            var user = await context.User.FindAsync(student.Id);
            decimal initial = 0;
            decimal expected1 = 400;
            decimal expected2 = 800;
            decimal expected3 = 400;

            //Initial Test, check User model is not null and tuition data is initialized correctly
            Assert.IsNotNull(user);
            Assert.AreEqual(initial, user.tuitionDue, "Initial Test");


            //Test 1
            var result1 = await regPageModel.OnPostRegistrationAsync(1);
            user = await context.User.FindAsync(student.Id);
            Assert.IsInstanceOfType(result1, typeof(RedirectToPageResult)); // Ensure the result is a redirect
            Assert.AreEqual(expected1, user.tuitionDue, "Test 1");

            //Test 2
            var result2 = await regPageModel.OnPostRegistrationAsync(2);
            user = await context.User.FindAsync(student.Id);
            Assert.IsInstanceOfType(result2, typeof(RedirectToPageResult)); // Ensure the result is a redirect
            Assert.AreEqual(expected2, user.tuitionDue, "Test 2");

            //Test 3
            var result3 = await regPageModel.OnPostRegistrationAsync(1);
            user = await context.User.FindAsync(student.Id);
            Assert.IsInstanceOfType(result3, typeof(RedirectToPageResult)); // Ensure the result is a redirect
            Assert.AreEqual(expected3, user.tuitionDue, "Test 3");
        }
    }
}