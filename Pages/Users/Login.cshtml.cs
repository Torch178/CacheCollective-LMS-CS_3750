using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesMovie.Models;
using RazorPagesMovie.Data;
using RazorPagesMovie.Pages.Movies;
using RazorPagesMovie.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace RazorPagesMovie.Pages.Users
{
    public class LoginModel : PageModel
    {
        private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;

        public LoginModel(RazorPagesMovieContext context)
        {
            _context = context;
        }

        [BindProperty]
        public LoginViewModel User { get; set; } = default!;

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid == false)
            {
                return Page();
            }

            var userExists = await _context.User.FirstOrDefaultAsync(u => u.Email == User.Email);
            if (userExists == null) // user doesn't exists in database
            {
                ModelState.AddModelError(string.Empty, "User doesn't exist.");
                return Page();
            }

            var validCredentials = userExists.Password == User.Password;
            if (validCredentials == false) // user's password doesn't match
            {
                ModelState.AddModelError(string.Empty, "Password is incorrect.");
                return Page();
            }

            return Redirect("/Users/" + userExists.Id);
        }
    }
}
