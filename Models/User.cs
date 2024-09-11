using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RazorPagesMovie.Models
{
    public class User
    {
        public int Id { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9]+@+[a-z]+.+[a-z]")]
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }

        [StringLength(20, MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }

        [Display(Name = "First Name")]
        [StringLength(20, MinimumLength = 1)]
        [RegularExpression(@"^[a-zA-Z]*")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(30, MinimumLength = 1)]
        [RegularExpression(@"^[a-zA-Z]*")]
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        public DateTime Birthdate { get; set; }
    }
}
