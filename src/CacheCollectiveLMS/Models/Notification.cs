using System.ComponentModel.DataAnnotations;

namespace RazorPagesMovie.Models
{
    public class Notification
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public int AssignmentId { get; set; }
        public DateTime DateCreated { get; set; }

        public string? Description { get; set; }
        public bool isRead { get; set; }
    }
}
