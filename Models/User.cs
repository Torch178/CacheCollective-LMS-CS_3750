using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RazorPagesMovie.Attributes;

namespace RazorPagesMovie.Models
{
    public class User
    {
        //Personal Information---------------------------------------------------------
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

        //Profile Pic----------------------
        [Display(Name = "Profile Picture")]
        public string? ProfilePic { get; set; } = "~/Images/Profile_Pics/default.png";

        //User Address Information---------------------------------------------------------
        [Display(Name = "Street Address")]
        [StringLength(256, MinimumLength = 1)]
        [RegularExpression(@"^[a-zA-Z0-9\s._'-]*")]
        [Required]
        public string StreetAddress {  get; set; }

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
        public string State {  get; set; }

        [Display(Name = "Zip Code")]
        [StringLength(10, MinimumLength = 1)]
        [RegularExpression(@"^(([0-9][0-9][0-9][0-9][0-9][-][0-9][0-9][0-9][0-9])|[0-9][0-9][0-9][0-9][0-9])")]
        [Required]
        public string Zip { get; set; }

      

    }
}