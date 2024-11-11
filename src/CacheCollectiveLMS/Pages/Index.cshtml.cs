using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;
using System.Security.Claims;
using RazorPagesMovie.Services;
using System.Text.Json;

namespace RazorPagesMovie.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;
        private readonly NotificationService _notificationService;

        public IndexModel(ILogger<IndexModel> logger, RazorPagesMovieContext context, NotificationService notificationService)
        {
            _logger = logger;
            _context = context;
            _notificationService = notificationService;
        }

        public User CurrentUser { get; set; }

        public async Task<IActionResult> OnGet()
        {
            if (CurrentUser == null)
            {
                // Fetch user from claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null) { return RedirectToPage("/Users/Login"); }
                if (!int.TryParse(userIdClaim, out var userId)) { return RedirectToPage("/Users/Login"); } // invalid userId

                var user = await _context.User.FirstOrDefaultAsync(m => m.Id == userId);
                if (user == null) { return RedirectToPage("/Users/Login"); }

                CurrentUser = user;

                IList<Models.Course> Courses;
                if (user.IsInstructor == false)
                {
                    Courses = await _context.Enrollment.Where(e => e.UserId == user.Id).Join(_context.Course, enrollment => enrollment.CourseId, course => course.CourseId, (enrollment, course) => course).ToListAsync();
                }
                else
                {
                    Courses = await _context.Course.Where(c => c.InstructorCourseId == user.Id).ToListAsync();
                }
                HttpContext.Session.SetString("IsInstructor", user.IsInstructor.ToString());
                HttpContext.Session.SetString("Courses", JsonSerializer.Serialize(Courses));
            }

            return Redirect("/Users/Index");
        }

        public async Task<IActionResult> OnPostMarkAsReadAsync(int notificationId)
        {
            await _notificationService.MarkNotificationAsReadAsync(notificationId);

            var notification = await _context.Notification.Where(n => n.Id == notificationId).FirstOrDefaultAsync();
            if (notification == null)
            {
                return RedirectToPage("/Index");
            }

            return RedirectToPage("/Course/Assignment/Details", new { id = notification.AssignmentId });
        }
    }
}