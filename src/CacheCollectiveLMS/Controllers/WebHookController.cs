using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RazorPagesMovie.Models;
using Stripe;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stripe.Checkout;
using Newtonsoft;
using RazorPagesMovie.Pages.Payments;
using Microsoft.AspNetCore.Authorization;


namespace RazorPagesMovie.Controllers
{
    
    [ApiController]
    [AllowAnonymous]
    [Route("[controller]/[action]")]
    public class WebHookController : ControllerBase
    {
        private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;

        public WebHookController(RazorPagesMovie.Data.RazorPagesMovieContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(string successMsg)
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            Session data = JsonConvert.DeserializeObject<Session>(json);
            return Ok(data);
        }

        const string endpointSecret = "whsec_2668db2175ef44784c4016a70012468bf73deab17c5e89972263cff051426d73";
        [HttpPost]
        public async Task<IActionResult> RecieveRequest()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) { return NotFound(); ; }
            if (!int.TryParse(userIdClaim, out var userId)) { return NotFound(); ; } // invalid userId

            var user = await _context.User.FirstOrDefaultAsync(m => m.Id == userId);
            if (user == null) { return NotFound(); }

            
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            Session data = JsonConvert.DeserializeObject<Session>(json);
            try
            {
                var stripeEvent = EventUtility.ParseEvent(json);
                var signatureHeader = Request.Headers["Stripe-Signature"];
                stripeEvent = EventUtility.ConstructEvent(json,
                    signatureHeader, endpointSecret);
                if (data == null)
                {
                    Console.WriteLine("Error: Deserialized Session Object is Null");
                    return BadRequest();
                }
                // Handle the event
                if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded || stripeEvent.Type == EventTypes.CheckoutSessionCompleted)
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    // Then define and call a method to handle the successful payment intent.
                    // handlePaymentIntentSucceeded(paymentIntent) - Not implemented yet;
                    //Collect Data for PaymentDetails constructor
                    int id = user.Id;
                    long? amtPaid = data.AmountTotal;
                    string payMethod = data.PaymentIntent.PaymentMethod.Card.Last4;
                    DateTime payDate = DateTime.Now;
                    int status = 0;

                    await user.payTuition(amtPaid, _context);

                    switch (paymentIntent.Status)
                    {
                        case ("succeeded"):
                            if (user.refundAmt != null && user.refundAmt > 0) status = 3;
                            else status = 0;
                            break;
                        case ("processing"):
                            status = 1;
                            break;
                        case ("cancelled"):
                            status = 2;
                            break;
                        default:
                            status = 4;
                            break;
                    }

                    PaymentDetails pd = new PaymentDetails(id, amtPaid, payDate, payMethod);
                    _context.PaymentDetails.Add(pd);
                    await _context.SaveChangesAsync();

                    //Constructors can only take arguments which directly correlate to entity class fields in C#, so a separate function
                    //is run to update the paystatus of PaymentDetails object, much like the updateTuition function for the User class.
                    //It is initialized as pending in the initial constructor method.
                    await pd.SetPayStatus(status, _context);
                    

                    //return RedirectToPage("./Payments/Success/{id}", pd.Id);
                }
                else if (stripeEvent.Type == EventTypes.PaymentMethodAttached)
                {
                    var paymentMethod = stripeEvent.Data.Object as PaymentMethod;
                    // Then define and call a method to handle the successful attachment of a PaymentMethod.
                    // handlePaymentMethodAttached(paymentMethod);
                }
                // ... handle other event types
                else
                {
                    // Unexpected event type
                    Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                }
                //return RedirectToPage("./Payments/Success");
                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }
        }
    }
}

