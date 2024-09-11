using System.ComponentModel.DataAnnotations;

namespace RazorPagesMovie.ViewModels
{
    public class LoginViewModel
    {
        [RegularExpression(@"^[a-zA-Z0-9]+@+[a-z]+.+[a-z]")]
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }

        [StringLength(20, MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }
    }
}
