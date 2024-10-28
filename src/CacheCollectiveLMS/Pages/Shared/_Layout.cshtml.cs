using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Linq;
using RazorPagesMovie.Models;

namespace RazorPagesMovie.Pages.Shared
{
    public class LayoutModel : PageModel
    {
        private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;

        public LayoutModel(RazorPagesMovie.Data.RazorPagesMovieContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User CurrentUser { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Console.WriteLine("Layout Loaded");
            if (CurrentUser == null)
            {
                // Fetch user from claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null) { return RedirectToPage("./Login"); }
                if (!int.TryParse(userIdClaim, out var userId)) { return RedirectToPage("./Login"); } // invalid userId

                var user = await _context.User.FirstOrDefaultAsync(m => m.Id == userId);
                if (user == null) { return NotFound(); }

                CurrentUser = user;
            }

            ViewData["IsInstructor"] = CurrentUser.IsInstructor;
            Console.WriteLine(ViewData["IsInstructor"]);

            return Page();
        }
    }
}
