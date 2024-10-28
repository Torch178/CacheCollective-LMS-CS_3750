using Microsoft.AspNetCore.Mvc;

namespace RazorPagesMovie.ViewModels
{
    public class PaymentViewModel
    {
        public string ID { get; set; }
        public decimal charge { get; set; }
    }
}
