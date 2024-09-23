using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity; // New directive for hashing password
using RazorPagesMovie.Models;
using RazorPagesMovie.Data;

namespace RazorPagesMovie.Pages.Users
{
    public class CreateModel : PageModel
    {
        private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;

        public CreateModel(RazorPagesMovieContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public User User { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid == false)
            {
                return Page();
            }

            var users = from u in _context.User select u;
            users = users.Where(u => u.Email == User.Email);

            if (users.Any() == true) // prevent duplicate emails from being created
            {
                ModelState.AddModelError(string.Empty, "Email already registered in database.");
                return Page();
            }
            else
            {
                // Password hashing logic before saving the user
                var passwordHasher = new PasswordHasher<User>();
                User.Password = passwordHasher.HashPassword(User, User.Password);

                _context.User.Add(User);
                await _context.SaveChangesAsync();

                return RedirectToPage("./Index");
            }
        }
    }
}
