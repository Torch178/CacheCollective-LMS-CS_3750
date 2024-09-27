using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;

namespace RazorPagesMovie.Pages.Course
{
    public class CourseListModel : PageModel
    {
        private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;

        public CourseListModel(RazorPagesMovie.Data.RazorPagesMovieContext context)
        {
            _context = context;
        }

        public IList<RazorPagesMovie.Models.Course> Course { get;set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {

            // Fetch user from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
            {
                return RedirectToPage("/Login"); // Redirect to login if user is not found
            }

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return RedirectToPage("/Login"); // Redirect if the user ID is invalid
            }

            // Retrieve the current logged-in user
            var user = await _context.User.FirstOrDefaultAsync(m => m.Id == userId);

            if (user == null || !user.IsInstructor)
            {
                return NotFound(); // Handle case where user is not found or not an instructor
            }

            // Retrieve courses for the current instructor
            Course = await _context.Course.Where(c => c.Instructor == $"{user.FirstName} {user.LastName}").ToListAsync();

            return Page();

        }
    }
}
