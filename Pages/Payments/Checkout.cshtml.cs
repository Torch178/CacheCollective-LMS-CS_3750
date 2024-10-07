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

        public class Startup
        {

            public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            {
                // This test secret API key is a placeholder. Don't include personal details in requests with this key.
                // To see your test secret API key embedded in code samples, sign in to your Stripe account.
                // You can also find your test secret API key at https://dashboard.stripe.com/test/apikeys.
                StripeConfiguration.ApiKey = "sk_test_4eC39HqLyjWDarjtT1zdp7dc";
                if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
                app.UseRouting();
                app.UseStaticFiles();
                app.UseEndpoints(endpoints => endpoints.MapControllers());
            }
        }

        public void OnGet()
        {
        }

        

        public IActionResult OnPost()
        {
            StripeConfiguration.ApiKey = "sk_test_4eC39HqLyjWDarjtT1zdp7dc";
            var domain = "http://localhost:7257";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                // Provide the exact Price ID (for example, pr_1234) of the product you want to sell
                Price = "{{PRICE_ID}}",
                Quantity = 1,
                },
            },
                Mode = "payment",
                SuccessUrl = domain + "/success.html",
                CancelUrl = domain + "/cancel.html",
            };
            var service = new SessionService();
            Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
        public IActionResult Create()
        {
            
            var domain = "http://localhost:7257";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                // Provide the exact Price ID (for example, pr_1234) of the product you want to sell
                Price = "{{PRICE_ID}}",
                Quantity = 1,
                },
            },
                Mode = "payment",
                SuccessUrl = domain + "/success.html",
                CancelUrl = domain + "/cancel.html",
            };
            var service = new SessionService();
            Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
        
    }
}
