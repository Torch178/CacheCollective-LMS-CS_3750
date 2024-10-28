using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RazorPagesMovie.Models;
using Stripe;
using Stripe.Checkout;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.Build.Framework;
using System.Net.Quic;
using RazorPagesMovie.ViewModels;
using Stripe.Climate;

namespace RazorPagesMovie.Pages.Payments
{
    public class CheckoutModel : PageModel
    {
        private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;

        public CheckoutModel(RazorPagesMovie.Data.RazorPagesMovieContext context)
        {
            _context = context;
        }

        public class StripeOptions
        {
            public string option { get; set; }
        }

        [BindProperty]
        public PaymentViewModel ViewModel { get; set; }
        public User CurrentUser { get; set; }
        public decimal? price { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) { return RedirectToPage("./Login"); }
            if (!int.TryParse(userIdClaim, out var userId)) { return RedirectToPage("./Login"); } // invalid userId

            var user = await _context.User.FirstOrDefaultAsync(m => m.Id == userId);
            if (user == null) { return NotFound(); }

            CurrentUser = user;
            price = (CurrentUser.tuitionDue - CurrentUser.tuitionPaid);

            ViewModel = new PaymentViewModel()
            {
                charge = (decimal)price,
                ID = CurrentUser.TuitionId

            };

            return Page();
        }

  
        public IActionResult OnPost()
        {

            StripeConfiguration.ApiKey = "acct_1Q6simP6Fkhfsw4o";
            var domain = "https://localhost:7257";

            var priceCreateOptions = new PriceCreateOptions
            {
                Product = ViewModel.ID,
                UnitAmountDecimal = ViewModel.charge * 100,
                Currency = "usd",
            };

            var priceService = new PriceService();
            var new_price = priceService.Create(priceCreateOptions);

            var options = new Stripe.Checkout.SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                    new Stripe.Checkout.SessionLineItemOptions
                    {
                    // Provide the exact Price ID (for example, pr_1234) of the product you want to sell
                 
                        Price = new_price.Id,
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = domain + "/Payments/Success",
                CancelUrl = domain + "/Payments/Cancel",
            };
            var service = new SessionService();

            Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
        
        
    }
}
