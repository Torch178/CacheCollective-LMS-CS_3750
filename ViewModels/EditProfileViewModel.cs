using RazorPagesMovie.Attributes;
using System.ComponentModel.DataAnnotations;

namespace RazorPagesMovie.ViewModels
{
    public class EditProfileViewModel
    {
        public int Id { get; set; }

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

        //User Address Information
        [Display(Name = "Street Address")]
        [StringLength(256, MinimumLength = 1)]
        [RegularExpression(@"^[a-zA-Z0-9\s._'-]*")]
        [Required]
        public string StreetAddress { get; set; }

        [Display(Name = "Apartment Number")]
        [StringLength(10, MinimumLength = 1)]
        [RegularExpression(@"^[a-zA-Z0-9]*")]
        public string ?ApartmentNum { get; set; }

        [Display(Name = "City")]
        [StringLength(256, MinimumLength = 1)]
        [RegularExpression(@"^[a-zA-Z0-9\s'-]*")]
        [Required]
        public string City { get; set; }

        [Display(Name = "State")]
        [StringLength(256, MinimumLength = 1)]
        [RegularExpression(@"^[a-zA-Z\s'-]*")]
        [Required]
        public string State { get; set; }

        [Display(Name = "Zip Code")]
        [StringLength(10, MinimumLength = 1)]
        [RegularExpression(@"^(([0-9][0-9][0-9][0-9][0-9][-][0-9][0-9][0-9][0-9])|[0-9][0-9][0-9][0-9][0-9])")]
        [Required]
        public string Zip { get; set; }
    }
}
