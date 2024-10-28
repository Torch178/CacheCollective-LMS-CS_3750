using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;
using System.Security.Claims;

namespace RazorPagesMovie.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;

        public IndexModel(ILogger<IndexModel> logger, RazorPagesMovieContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> OnGet()
        {
            // Fetch user from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) { return RedirectToPage("/Users/Login"); }
            if (!int.TryParse(userIdClaim, out var userId)) { return RedirectToPage("/Users/Login"); } // invalid userId

            var user = await _context.User.FirstOrDefaultAsync(m => m.Id == userId);
            if (user == null) { return RedirectToPage("/Users/Login"); }

            HttpContext.Session.SetString("IsInstructor", user.IsInstructor.ToString());
            return Redirect("/Users/Index");
        }
    }
}
