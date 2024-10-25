using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stripe;
using Stripe.Checkout;

namespace RazorPagesMovie.Pages.Payments
{
    public class CheckoutModel : PageModel
    {
        public class StripeOptions
        {
            public string option { get; set; }
        }

        

        public void OnGet()
        {
        }

        

        public IActionResult OnPost()
        {
            StripeConfiguration.ApiKey = "sk_test_51Q6simP6Fkhfsw4osozLyQK35jEf9YNVBsyRSyEN80Colog02BNiuhb4lg4wNN604wapVshVJvi7D5JINpJGiogY00fJyblERC";
            //"https://localhost:7257";
            var domain = "https://localhost:7257";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>

            {
                new SessionLineItemOptions
                {
                // Provide the exact Price ID (for example, pr_1234) of the product you want to sell
                 
                Price = "price_1Q99SrP6Fkhfsw4ollDuBi6d",
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
