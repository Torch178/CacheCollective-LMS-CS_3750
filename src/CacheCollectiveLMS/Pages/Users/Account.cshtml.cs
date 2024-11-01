using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RazorPagesMovie.Models;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace RazorPagesMovie.Pages.Users
{
    public class AccountModel : PageModel
    {
        private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;

        public AccountModel(RazorPagesMovie.Data.RazorPagesMovieContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User CurrentUser { get; set; }
        public string msg { get; set; } = "";
        public bool balanceDue { get; set; }
        public bool prevPayments { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            // Fetch user from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) { return RedirectToPage("./Login"); }
            if (!int.TryParse(userIdClaim, out var userId)) { return RedirectToPage("./Login"); } // invalid userId

            var user = await _context.User.FirstOrDefaultAsync(m => m.Id == userId);
            if (user == null) { return NotFound(); }
            CurrentUser = user;
            var owed = CurrentUser.GetBalance();

            // ***! - TODO Fix Later - !*** Throws error when fetching list of PaymentDetails from DB context. Throws 
            //Error for converting from Int64 to Decimal Data type when querying database for some reason
            //IList<PaymentDetails> payments = await _context.PaymentDetails.Where(e => e.userId == user.Id).ToListAsync();
            prevPayments = false;

            if (owed > 0) { balanceDue = true; msg = "Remaining Balance Due: $" + owed.ToString(); }
            else if (owed < 0) { balanceDue = false; msg = "You are owed a refund of $" + user.refundAmt.ToString() + "."; }
            else { balanceDue = false; msg = "Tuition fully paid. No upcoming payments due."; }

            return Page();
        }
    }
}