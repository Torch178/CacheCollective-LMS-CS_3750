using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using RazorPagesMovie.Models;
using System.Configuration;
using System.Security.Claims;

namespace RazorPagesMovie.Pages.Users
{

    public class IndexModel : PageModel
    {
        private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;

        public IndexModel(RazorPagesMovie.Data.RazorPagesMovieContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User CurrentUser { get; set; } = default!;
        [BindProperty]
        public string Layout { get; set; } = "_Layout";
        [BindProperty]
        public IList<Models.Course> Course { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            // Fetch user from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) { return RedirectToPage("./Users/Login"); }
            if (!int.TryParse(userIdClaim, out var userId)) { return RedirectToPage("./Users/Login"); } // invalid userId

            var user = await _context.User.FirstOrDefaultAsync(m => m.Id == userId);
            if (user == null) { return NotFound(); }

            CurrentUser = user;
            Course = await _context.Enrollment.Where(e => e.UserId == user.Id).Join(_context.Course, enrollment => enrollment.CourseId, course => course.CourseId, (enrollment, course) => course).ToListAsync();

            return Page();
        }
    }
}
