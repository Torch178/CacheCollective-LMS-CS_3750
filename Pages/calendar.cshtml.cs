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
            if (!int.TryParse(userIdClaim, out var userId)) { return RedirectToPage("./Login"); } // invalid userId

            var user = await _context.User.FirstOrDefaultAsync(m => m.Id == userId);
            if (user == null) { return NotFound(); }



            // If the user is not found, handle accordingly
            if (user == null)
            {
                Events.Add(new { title = "User not found", start = "2024-09-24", end = "2024-09-24" });
                return Page();
            }

            // Check if the user is an instructor and add events accordingly
            if (user.IsInstructor == true)
            {
                // Add events for instructors
                Events.Add(new { title = "Instructor Meeting", start = "2024-09-25", end = "2024-09-25" });
                Events.Add(new { title = "Course Planning", start = "2024-09-28", end = "2024-09-28" });
                Events.Add(new { title = "Grading Session", start = "2024-09-29", end = "2024-09-29" });
            }
            else
            {
                // Add events for students
                Events.Add(new { title = "Class Lecture", start = "2024-09-25", end = "2024-09-25" });
                Events.Add(new { title = "Study Group", start = "2024-09-28", end = "2024-09-28" });
                Events.Add(new { title = "Exam Review", start = "2024-09-29", end = "2024-09-29" });
            }

            return Page();
        }
    }
}
