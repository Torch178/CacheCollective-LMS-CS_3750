using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using RazorPagesMovie.Attributes;

namespace RazorPagesMovie.Models
{
    public class User
    {
        public int Id { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9]+@+[a-z]+.+[a-z]")]
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }

        [StringLength(255)] // Increase the maximum length to 255 to incorporate the hashing
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }

        [Display(Name = "First Name")]
        [StringLength(20, MinimumLength = 1)]
        [RegularExpression(@"^[a-zA-Z]*")]
        [Required]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(30, MinimumLength = 1)]
        [RegularExpression(@"^[a-zA-Z]*")]
        [Required]
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        [MinAge(16)]
        [Required]
        public DateTime Birthdate { get; set; }

        [Display(Name = "Are you an instructor?")]
        [Required(ErrorMessage = "Please select either Yes or No.")]
        public bool IsInstructor { get; set; }
    }
}