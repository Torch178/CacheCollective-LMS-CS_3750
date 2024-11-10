using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesMovie.Models;
using RazorPagesMovie.Data;
using RazorPagesMovie.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using System.Text.Json;

namespace RazorPagesMovie.Pages.Users
{
    public class LoginModel : PageModel
    {
        private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginModel(RazorPagesMovieContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
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

            // Use PasswordHasher to verify the password
            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(userExists, userExists.Password, User.Password);
            if (result != PasswordVerificationResult.Success) // updated for hashing password
            {
                ModelState.AddModelError(string.Empty, "Password is incorrect.");
                return Page();
            }

            // Sign in with claim-based authorization
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userExists.Id.ToString()),
                new Claim(ClaimTypes.Email, userExists.Email),
                new Claim("FirstName", userExists.FirstName),
                new Claim("LastName", userExists.LastName),
                new Claim("IsInstructor", userExists.IsInstructor.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(userClaims, "User");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            var claimsAuthenticationProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1)
            };
            await _httpContextAccessor.HttpContext.SignInAsync(claimsPrincipal, claimsAuthenticationProperties);


            IList<Models.Course> Courses;
            if (userExists.IsInstructor == false)
            {
                Courses = await _context.Enrollment.Where(e => e.UserId == userExists.Id).Join(_context.Course, enrollment => enrollment.CourseId, course => course.CourseId, (enrollment, course) => course).ToListAsync();
            }
            else
            {
                Courses = await _context.Course.Where(c => c.InstructorCourseId == userExists.Id).ToListAsync();
            }
            HttpContext.Session.SetString("Courses", JsonSerializer.Serialize(Courses));
            HttpContext.Session.SetString("IsInstructor", userExists.IsInstructor.ToString());

            return RedirectToPage("./Index");
        }
    }
}