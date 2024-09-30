// Logout.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesMovie.Pages.Users
{
    [Authorize]
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGet()
        {
            // Sign out the user
            await HttpContext.SignOutAsync("ClaimBasedSchema");

            HttpContext.Session.SetString("IsInstructor", "NULL");

            // Redirect to the login page after logout
            return RedirectToPage("/Users/Login");
        }
    }
}