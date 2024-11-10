using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity; // New directive for hashing password
using RazorPagesMovie.Models;
using RazorPagesMovie.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Stripe;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace RazorPagesMovie.Pages.Users
{
    public class CreateModel : PageModel
    {
        private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateModel(RazorPagesMovieContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
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

                if (!User.IsInstructor)
                {
                    User.tuitionDue = 0;
                    User.tuitionPaid = 0;
                    User.refundAmt = 0;
                    User.TuitionId = Guid.NewGuid().ToString();

                    //Create unique tuition product for student to make payments
                    StripeConfiguration.ApiKey = "sk_test_51Q6simP6Fkhfsw4osozLyQK35jEf9YNVBsyRSyEN80Colog02BNiuhb4lg4wNN604wapVshVJvi7D5JINpJGiogY00fJyblERC";
                    var options = new ProductCreateOptions { Name = "Tuition_" + User.Id.ToString(), Id = User.TuitionId };
                    var service = new ProductService();
                    service.Create(options);
                }

                _context.User.Add(User);
                await _context.SaveChangesAsync();

                // Sign in with claim-based authorization
                var userClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, User.Id.ToString()),
                    new Claim(ClaimTypes.Email, User.Email),
                    new Claim("FirstName", User.FirstName),
                    new Claim("LastName", User.LastName),
                    new Claim("IsInstructor", User.IsInstructor.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(userClaims, "User");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await _httpContextAccessor.HttpContext.SignInAsync(claimsPrincipal);

                // Update HTTP Session State Courses
                IList<Models.Course> Courses;
                if (User.IsInstructor == false)
                {
                    Courses = await _context.Enrollment.Where(e => e.UserId == User.Id).Join(_context.Course, enrollment => enrollment.CourseId, course => course.CourseId, (enrollment, course) => course).ToListAsync();
                }
                else
                {
                    Courses = await _context.Course.Where(c => c.InstructorCourseId == User.Id).ToListAsync();
                }
                HttpContext.Session.SetString("Courses", JsonSerializer.Serialize(Courses));
                HttpContext.Session.SetString("IsInstructor", User.IsInstructor.ToString());

                return RedirectToPage("./Index");
            }
        }
    }
}
