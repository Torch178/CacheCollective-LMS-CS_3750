using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using RazorPagesMovie.Models;
using System.Configuration;

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
        public User User { get; set; } = default!;
        [BindProperty]
        public string layout { get; set; } = "_Layout";
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null) { return NotFound(); }

            User = user;

            //load appropriate layouts based on user data
            if (user.IsInstructor) layout = "_Layout_Instructor";
            else if (!user.IsInstructor) layout = "_Layout_Student";

            return Page();
        }
    }
}
