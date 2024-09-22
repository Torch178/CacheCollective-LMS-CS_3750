using System.ComponentModel.DataAnnotations;

namespace RazorPagesMovie.Models
{
    public class Course
    {
        // Course Id for primary key
        public int CourseId { get; set; }

        // Rows to have
        // Department - select out of options
        [Required]
        public Department Department { get; set; }

        // Course Number
        [Required]
        public int Number { get; set; }

        // Course Title
        [Required]
        public string Title { get; set; }

        // Student Capacity
        [Required]
        public int Capacity { get; set; }

        [Required]
        [Range(1, 5)]  // Assuming typical credit hours are between 1 and 5
        public short CreditHours { get; set; }


        public string MeetingDays { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }  // Start time for class

        [Required]
        public TimeSpan EndTime { get; set; }  // End time for class

        // Meeting Location
        [Required]
        public string Location { get; set; }

        // Instructor teaching the course. Possibly nullable if they are uncertain of the teacher currently
        public string? Instructor { get; set; }

    }
}
