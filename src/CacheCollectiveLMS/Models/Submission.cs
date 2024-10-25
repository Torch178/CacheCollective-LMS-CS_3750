using System.ComponentModel.DataAnnotations.Schema;

namespace RazorPagesMovie.Models
{
    public class Submission
    {
        public int SubmissionId { get; set; }
        public int AssignmentId { get; set; } // Foreign key to the assignment
        public int UserId { get; set; } // Foreign key to the student who submitted
        public SubmissionType SubmissionType { get; set; } // Enum to specify text or file

        // Properties for text-based submissions
        public string? SubmittedText { get; set; }

        // Properties for file-based submissions
        public string? FilePath { get; set; }
        public string? FileName { get; set; } // For storing original file name
        public DateTime SubmissionDate { get; set; } // Date and time of submission

        public double? GradedPoints { get; set; } = null;
        [NotMapped]
        public Assignment? Assignment { get; set; }

        public string? InstructorComments { get; set; } // For storing instructor comments

    }
}
