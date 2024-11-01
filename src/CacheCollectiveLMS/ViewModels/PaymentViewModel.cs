using Microsoft.AspNetCore.Mvc;
using RazorPagesMovie.Attributes;
using System.ComponentModel.DataAnnotations;

namespace RazorPagesMovie.ViewModels
{
    public class PaymentViewModel
    {
        public string ID { get; set; }

        [DataType(DataType.Currency)]
        public decimal charge { get; set; }
    }
}