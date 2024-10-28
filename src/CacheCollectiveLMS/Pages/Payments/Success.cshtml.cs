using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stripe;
using Stripe.Checkout;
using Newtonsoft;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using RazorPagesMovie.Models;

namespace RazorPagesMovie.Pages.Payments
{
    public class SuccessModel : PageModel
    {
        private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;

        public SuccessModel(RazorPagesMovie.Data.RazorPagesMovieContext context)
        {
            _context = context;
        }

        public string successMsg { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var pd = await _context.PaymentDetails.FindAsync(id);
            if (pd != null) { successMsg = "Success! Payment successfully processed. See Payment Details below. <br>" + pd.ToString(); }
            else successMsg = string.Empty;
            return Page();
        }   
    }
}
