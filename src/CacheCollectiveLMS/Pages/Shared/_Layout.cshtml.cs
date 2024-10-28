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

        public async Task<IActionResult> OnGetAsync()
        {
            return Page();
        }
    }
}