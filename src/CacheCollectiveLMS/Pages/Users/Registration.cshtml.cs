using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Models;
using System.Security.Claims;
using System.Text.Json;

namespace RazorPagesMovie.Pages.Users
{
    public class RegistrationModel : PageModel
    {
        public readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;

        public RegistrationModel(RazorPagesMovie.Data.RazorPagesMovieContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User CurrentUser { get; set; }
        [BindProperty]
        public IList<Models.Course> Course { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (User.Identity.IsAuthenticated && Int32.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out var userId)) // get logged-in user's claims
            {
                CurrentUser = await _context.User.FirstOrDefaultAsync(u => u.Id == userId);
            }
            Course = await _context.Course.ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostRegistrationAsync(int courseId)
        {
            if (User.Identity.IsAuthenticated && Int32.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out var userId)) // get logged-in user's claims
            {
                CurrentUser = await _context.User.FirstOrDefaultAsync(u => u.Id == userId);
            }
            if (CurrentUser == null || CurrentUser.Id == 0) { return NotFound(); }

            var enrollmentExists = await _context.Enrollment.FirstOrDefaultAsync(e => e.UserId == CurrentUser.Id && e.CourseId == courseId);
            if (enrollmentExists == null)
            {
                var enrollment = new Enrollment
                {
                    CourseId = courseId,
                    UserId = CurrentUser.Id
                };
                _context.Enrollment.Add(enrollment);
            }
            else
            {
                _context.Enrollment.Remove(enrollmentExists);
            }

            await _context.SaveChangesAsync();
            await CurrentUser.updateTuition(_context);

            // Update HTTP Session State Courses
            IList<Models.Course> Courses;
            if (CurrentUser.IsInstructor == false)
            {
                Courses = await _context.Enrollment.Where(e => e.UserId == CurrentUser.Id).Join(_context.Course, enrollment => enrollment.CourseId, course => course.CourseId, (enrollment, course) => course).ToListAsync();
            }
            else
            {
                Courses = await _context.Course.Where(c => c.InstructorCourseId == CurrentUser.Id).ToListAsync();
            }
            HttpContext.Session.SetString("Courses", JsonSerializer.Serialize(Courses));

            return RedirectToPage();
        }
    }
}
