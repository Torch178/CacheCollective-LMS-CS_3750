using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

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
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) { return RedirectToPage("./Login"); }
            if (!int.TryParse(userIdClaim, out var userId)) { return RedirectToPage("./Login"); }

            var user = await _context.User.FirstOrDefaultAsync(m => m.Id == userId);
            if (user == null) { return NotFound(); }

            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek);

            if (user.IsInstructor)
            {
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
                                    title = $"{course.Title} {course.Number}",
                                    start = eventDate.Add(course.StartTime),
                                    url = Url.Page("/Course/Details", new { id = course.CourseId }),
                                    type = "course", // Mark it as a course event
                                });
                            }
                        }
                    }
                }
            }
            else
            {
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
                                        title = $"{course.Title} {course.Number}",
                                        start = eventDate.Add(course.StartTime),
                                        url = Url.Page("/Course/Details", new { id = course.CourseId }),
                                        type = "course", // Mark it as a course event
                                    });
                                }
                            }
                        }

                        var assignments = await _context.Assignment
                            .Where(a => a.CourseId == course.CourseId)
                            .ToListAsync();

                        foreach (var assignment in assignments)
                        {
                            Events.Add(new
                            {
                                title = $"{assignment.Title}",
                                start = assignment.DueDate,
                                end = assignment.DueDate.AddSeconds(1),
                                url = Url.Page("/Course/Assignment/Details", new { id = assignment.Id }),
                                type = "assignment", // Mark it as an assignment event
                            });
                        }
                    }
                }
            }

            return Page();
        }
    }
}
