using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;

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

        public IActionResult OnGet()
        {

            return Redirect("/Users/Login");
            
        }
    }
}
