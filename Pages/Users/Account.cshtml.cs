using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Models;
using System.Security.Claims;

namespace RazorPagesMovie.Pages.Users
{
    public class AccountModel : PageModel
    {
        private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;

        public AccountModel(RazorPagesMovie.Data.RazorPagesMovieContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User CurrentUser { get; set; }
        [BindProperty]
        public double TotalTuition { get; set; }
        [BindProperty]
        public List<RazorPagesMovie.Models.Course> UserCourses { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Fetch user from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) { return RedirectToPage("./Login"); }
            if (!int.TryParse(userIdClaim, out var userId)) { return RedirectToPage("./Login"); } // invalid userId

            var user = await _context.User.FirstOrDefaultAsync(m => m.Id == userId);
            if (user == null) { return NotFound(); }

            UserCourses = await _context.Enrollment.Where(e => e.UserId == user.Id).Join(_context.Course, enrollment => enrollment.CourseId, course => course.CourseId, (enrollment, course) => course).ToListAsync();
            CurrentUser = user;
            TotalTuition = UserCourses.Sum(c => c.CreditHours) * 100;

            return Page();
        }
    }
}
