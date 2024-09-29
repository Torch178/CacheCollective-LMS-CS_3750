using RazorPagesMovie.Attributes;
using System.ComponentModel.DataAnnotations;

namespace RazorPagesMovie.ViewModels
{
    public class EditProfileViewModel
    {
        public int Id { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9]+@+[a-z]+.+[a-z]")]
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }

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
    }
}
