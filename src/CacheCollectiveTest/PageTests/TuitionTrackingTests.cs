using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;
using RazorPagesMovie.Pages.Course;
using RazorPagesMovie.Pages.Payments;
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
        //This tests that the Tuition Due for a student account dynamically updates when registering(inc+) and dropping classes(dec-)
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

        //Template session class to mock a real session for session states
        public class TestSession : ISession
        {
            private readonly Dictionary<string, byte[]> _sessionStorage = new Dictionary<string, byte[]>();

            public IEnumerable<string> Keys => _sessionStorage.Keys;

            public bool IsAvailable => true;

            public string Id => Guid.NewGuid().ToString();

            public void Clear() => _sessionStorage.Clear();

            public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

            public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

            public void Remove(string key) => _sessionStorage.Remove(key);

            public void Set(string key, byte[] value) => _sessionStorage[key] = value;

            public bool TryGetValue(string key, out byte[] value) => _sessionStorage.TryGetValue(key, out value);

            public void SetString(string key, string value)
            {
                Set(key, Encoding.UTF8.GetBytes(value));
            }

            public string? GetString(string key)
            {
                return TryGetValue(key, out var value) ? Encoding.UTF8.GetString(value) : null;
            }
        }

        private async void InitTuitionDue(RazorPagesMovieContext context)
        {
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
        }
        private async void InitTuitionPaid(RazorPagesMovieContext context)
        {
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
                tuitionDue = 800,
                tuitionPaid = 0,
                refundAmt = 0,
                TuitionId = "01"
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
            var enrollment1 = new Enrollment
            {
                Id = 1,
                CourseId = 1,
                UserId = 1,
            };
            var enrollment2 = new Enrollment
            {
                Id = 2,
                CourseId = 2,
                UserId = 1,
            };
            context.User.Add(student);
            await context.SaveChangesAsync();
            context.Course.Add(course1);
            await context.SaveChangesAsync();
            context.Course.Add(course2);
            await context.SaveChangesAsync();
            context.Enrollment.Add(enrollment1);
            await context.SaveChangesAsync();
            context.Enrollment.Add(enrollment2);
            await context.SaveChangesAsync();
        }

        [TestMethod()] // Tuition Test 1 --------------
        public async Task TuitionDueUpdate_OnCourseReg_Test()
        {
            //Three steps in test methods
            // ---
            // Arrange
            // --- Set up the necessary objects, data, and conditions required for the test. This includes initializing variables, creating mock data, or setting up dependencies.
            var context = GetInMemoryContext();
            //ensure database is cleared and in a clean state (empty) for testing
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
            InitTuitionDue(context);

            var student = await context.User.FirstAsync();
            // Set up page model now
            var regPageModel = new RegistrationModel(context)
            {
                CurrentUser = await context.User.FirstAsync(),
                Course = await context.Course.ToListAsync()
            };

            // Simulate a logged in student
            regPageModel.PageContext.HttpContext = new DefaultHttpContext
            {
                User = CreateStudent(student.Id),
                Session = new TestSession()
            };
   
            // ---
            // Act
            // --- Perform the action or method you want to test. This is where you call the method and execute the logic being tested.
            // ---
            // Assert
            //  --- Verify that the outcome of the action is as expected. This involves checking results, validating state changes, or ensuring that the correct behavior occurred.
            var user = await context.User.FindAsync(1);
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
            Assert.AreEqual(expected1, user.tuitionDue, "Test 1 Failed, Didn't update tuitionDue upon course registration");
            Assert.AreEqual(0, user.tuitionPaid);
            Assert.AreEqual(0, user.refundAmt);

            //Test 2
            var result2 = await regPageModel.OnPostRegistrationAsync(2);
            user = await context.User.FindAsync(student.Id);
            Assert.IsInstanceOfType(result2, typeof(RedirectToPageResult)); // Ensure the result is a redirect
            Assert.AreEqual(expected2, user.tuitionDue, "Test 2 Failed, Didn't update tuitionDue upon course registration");
            Assert.AreEqual(0, user.tuitionPaid);
            Assert.AreEqual(0, user.refundAmt);
            //Test 3
            var result3 = await regPageModel.OnPostRegistrationAsync(1);
            user = await context.User.FindAsync(student.Id);
            Assert.IsInstanceOfType(result3, typeof(RedirectToPageResult)); // Ensure the result is a redirect
            Assert.AreEqual(expected3, user.tuitionDue, "Test 3 Failed, Didn't update tuitionDue upon dropping enrolled course");
            Assert.AreEqual(0, user.tuitionPaid);
            Assert.AreEqual(0, user.refundAmt);
        }
        
        
        [TestMethod()] // Tuition Test 2 --------------
        public async Task TuitionPaidUpdate_AfterCheckout_Test()
        {

            //Payment amt will be sent to Success page to test that tuitionPaid is being updated and payment is saved in PaymentDetails object

            //Three steps in test methods
            // ---
            // Arrange
            // --- Set up the necessary objects, data, and conditions required for the test. This includes initializing variables, creating mock data, or setting up dependencies.
            var context = GetInMemoryContext();
            //ensure database is cleared and in a clean state (empty) for testing
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
            InitTuitionPaid(context);
            var student = await context.User.FirstAsync();
            // Set up page model now
            var SuccessPageModel = new SuccessModel(context)
            {
                CurrentUser = await context.User.FirstAsync(),
            };

            // Simulate a logged in student
            SuccessPageModel.PageContext.HttpContext = new DefaultHttpContext
            {
                User = CreateStudent(student.Id)
            };

            // ---
            // Act
            // --- Perform the action or method you want to test. This is where you call the method and execute the logic being tested.
            // ---
            // Assert
            //  --- Verify that the outcome of the action is as expected. This involves checking results, validating state changes, or ensuring that the correct behavior occurred.
            var user = await context.User.FindAsync(1);
            var pd = await context.PaymentDetails.ToListAsync();
 
            //Initial Test, check User model is not null and tuition data is initialized correctly
            Assert.IsNotNull(user);
            Assert.AreEqual(0, user.tuitionPaid, "Initial Test");
            Assert.AreEqual((decimal?)800, user.tuitionDue, "Initial Test");
            Assert.AreEqual(0,pd.Count);

            //Test 4  
            var result1 = await SuccessPageModel.OnGetAsync((decimal?)400);
            user = await context.User.FindAsync(student.Id);
            pd = await context.PaymentDetails.ToListAsync();
            Assert.IsInstanceOfType(result1, typeof(PageResult)); // Ensure the result is a redirect
            Assert.IsNotNull(pd);
            Assert.AreEqual(1, pd.Count);
            Assert.AreEqual((decimal?)400, user.tuitionPaid, "Test 4");
            Assert.AreEqual((decimal?)800, user.tuitionDue);
            Assert.AreEqual((decimal?)400, user.GetBalance());
            //Test 5
            var result2 = await SuccessPageModel.OnGetAsync((decimal?)400);
            user = await context.User.FindAsync(student.Id);
            pd = await context.PaymentDetails.ToListAsync();
            Assert.IsNotNull(pd);
            Assert.AreEqual(2, pd.Count);
            Assert.IsInstanceOfType(result2, typeof(PageResult)); // Ensure the result is a redirect
            Assert.AreEqual((decimal?)800, user.tuitionPaid, "Test 5");
            Assert.AreEqual((decimal?)800, user.tuitionDue);
            Assert.AreEqual((decimal?)0, user.GetBalance());
            Assert.AreEqual(0, user.refundAmt);
            
        }
    }
}