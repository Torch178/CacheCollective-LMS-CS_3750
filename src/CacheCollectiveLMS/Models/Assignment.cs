using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace RazorPagesMovie.Models
{
    public class Assignment
    {
        //Assignment Id
        public int Id { get; set; }

        [Required]
        public int CourseId { get; set; }

        //Assignment title
        [Required(ErrorMessage = "Title is required")]
        [StringLength(50, ErrorMessage = "Title cannot be longer that 25 characters.")]
        public string Title { get; set; }

        //Description
        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        public string Description { get; set; }

        //Max points
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Max Points must be greater than zero.")]
        public int MaxPoints { get; set; }

        //Due date/time
        [Required(ErrorMessage = "Due date is required.")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Due Date")]
        [CustomValidation(typeof(Assignment), nameof(ValidateDueDateTime))]
        public DateTime DueDate { get; set; }
        [Required(ErrorMessage = "Submission type is required.")]
        //Submission type(text entry / file submission - drop down list)
        public SubmissionType SubmissionType { get; set; }

        public static ValidationResult ValidateDueDateTime(DateTime dueDate, ValidationContext context)
        {
            if (dueDate < DateTime.Now)
            {
                return new ValidationResult("Due date and time must be in the future.");
            }
            return ValidationResult.Success;
        }

    }
}
