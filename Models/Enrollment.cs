using System.ComponentModel.DataAnnotations;

namespace RazorPagesMovie.Models
{
    public class Enrollment
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        [Required]
        public int CourseId { get; set; }
    }
}
