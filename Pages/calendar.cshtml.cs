using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RazorPagesMovie.Pages
{
    public class calendarModel : PageModel
    {
        private readonly RazorPagesMovieContext _context;

        public calendarModel(RazorPagesMovieContext context)
        {
            _context = context;
        }

        public List<object> Events { get; set; } = new List<object>();

        [BindProperty]
        public User CurrentUser { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            // Fetch user from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) { return RedirectToPage("./Login"); }
            if (!int.TryParse(userIdClaim, out var userId)) { return RedirectToPage("./Login"); }

            var user = await _context.User.FirstOrDefaultAsync(m => m.Id == userId);
            if (user == null) { return NotFound(); }

            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek);

            if (user.IsInstructor)
            {
                // Fetch courses taught by the instructor
                var courses = await _context.Course
                    .Where(c => c.InstructorCourseId == user.Id)
                    .ToListAsync();

                foreach (var course in courses)
                {
                    foreach (var meetingDay in course.MeetingDays.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (Enum.TryParse(meetingDay.Trim(), out DayOfWeek dayOfWeek))
                        {
                            for (int weekOffset = 0; weekOffset < 15; weekOffset++)
                            {
                                var eventDate = startOfWeek.AddDays((int)dayOfWeek + (weekOffset * 7));
                                Events.Add(new
                                {
                                    title = $"{course.Title} {course.Number} {course.Location}",
                                    start = eventDate.Add(course.StartTime)
                                    //location = course.Location,
                                    //description = $"Taught by {user.FirstName} {user.LastName}"
                                });
                            }
                        }
                    }
                }
            }
            else
            {
                // Fetch enrollments for the user
                var enrollments = await _context.Enrollment
                    .Where(e => e.UserId == userId)
                    .ToListAsync();

                foreach (var enrollment in enrollments)
                {
                    var course = await _context.Course.FindAsync(enrollment.CourseId);
                    if (course != null)
                    {
                        foreach (var meetingDay in course.MeetingDays.Split(',', StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (Enum.TryParse(meetingDay.Trim(), out DayOfWeek dayOfWeek))
                            {
                                for (int weekOffset = 0; weekOffset < 15; weekOffset++)
                                {
                                    var eventDate = startOfWeek.AddDays((int)dayOfWeek + (weekOffset * 7));
                                    Events.Add(new
                                    {
                                        title = $"{course.Title} {course.Number} {course.Location}",
                                        start = eventDate.Add(course.StartTime)
                                        //location = course.Location,
                                        //description = "Enrolled in this course"
                                    });
                                }
                            }
                        }

                        // Fetch assignments for the course
                        var assignments = await _context.Assignment
                            .Where(a => a.CourseId == course.CourseId)
                            .ToListAsync();

                        foreach (var assignment in assignments)
                        {
                            Events.Add(new
                            {
                                title = $"Assignment: {assignment.Title}",
                                start = assignment.DueDate,
                                end = assignment.DueDate.AddSeconds(1) // Added one second to ensure the event remains on the intended due date
                                //location = course.Location, // Optional: Use course location for context
                                //description = assignment.Description // Ensure this is consistently provided
                            });
                        }
                    }
                }
            }

            return Page();
        }



    }
}
