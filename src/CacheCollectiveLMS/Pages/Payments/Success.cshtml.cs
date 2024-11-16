using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stripe;
using Stripe.Checkout;
using Newtonsoft;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using RazorPagesMovie.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RazorPagesMovie.Pages.Payments
{
    public class SuccessModel : PageModel
    {
        private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;

        public SuccessModel(RazorPagesMovie.Data.RazorPagesMovieContext context)
        {
            _context = context;
        }
        public User CurrentUser { get; set; }
        public string successMsg { get; set; }
        public PaymentDetails summary { get; set; }
        public async Task<IActionResult> OnGetAsync(decimal? amt)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) { return RedirectToPage("./Login"); }
            if (!int.TryParse(userIdClaim, out var userId)) { return RedirectToPage("./Login"); } // invalid userId

            var user = await _context.User.FirstOrDefaultAsync(m => m.Id == userId);
            if (user == null) { return NotFound(); }
            else CurrentUser = user;

            int id = CurrentUser.Id;
            decimal? amtPaid = amt;
            string payMethod = "4242";
            DateTime payDate = DateTime.Now;
            int status = 4;

            await CurrentUser.payTuition(amtPaid, _context);

            PaymentDetails pd = new PaymentDetails(id, amtPaid, payDate, payMethod);
            _context.PaymentDetails.Add(pd);
            await _context.SaveChangesAsync();

            //Constructors can only take arguments which directly correlate to entity class fields in C#, so a separate function
            //is run to update the paystatus of PaymentDetails object, much like the updateTuition function for the User class.
            //It is initialized as pending in the initial constructor method.
            await pd.SetPayStatus(status, _context);
            successMsg = string.Format("Tuition Payment of {0:C} successfully processed! Remaining balance is {1:C}", amt, CurrentUser.GetBalance());
            summary = pd;
            return Page();
        }
    }
}