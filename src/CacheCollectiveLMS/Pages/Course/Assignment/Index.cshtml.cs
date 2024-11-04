using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Models;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RazorPagesMovie.Pages.Course.Assignment
{
    public class IndexModel : PageModel
    {
        private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;

        public IndexModel(RazorPagesMovie.Data.RazorPagesMovieContext context)
        {
            _context = context;
        }

        public IList<RazorPagesMovie.Models.Assignment> Assignments { get; set; } = new List<RazorPagesMovie.Models.Assignment>();
        public Dictionary<int, double?> Grades { get; set; } = new Dictionary<int, double?>();

        [BindProperty(SupportsGet = true)]
        public int CourseId { get; set; }

        public string CourseName { get; set; }

        public double TotalPoints { get; set; }
        public double TotalGrades { get; set; }
        public double TotalPercentage { get; set; }
        public string LetterGrade { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var course = await _context.Course.FirstOrDefaultAsync(c => c.CourseId == CourseId);
            if (course == null)
            {
                return NotFound();
            }

            CourseName = course.Title;

            Assignments = await _context.Assignment
                .Where(a => a.CourseId == CourseId)
                .ToListAsync();

            // Fetch user from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
            {
                return RedirectToPage("/Users/Login");
            }

            // Get the grades for the user's submissions
            var submissions = await _context.Submission
                .Where(s => Assignments.Select(a => a.Id).Contains(s.AssignmentId) && s.UserId == userId)
                .ToListAsync();

            foreach (var submission in submissions)
            {
                if (submission.GradedPoints.HasValue)
                {
                    Grades[submission.AssignmentId] = submission.GradedPoints.Value;
                }
            }

            // Calculate Total Points and Total Grades
            TotalPoints = Assignments.Sum(a => a.MaxPoints);
            TotalGrades = Grades.Values.Where(g => g.HasValue).Sum(g => g.Value);

            // Calculate Total Percentage
            TotalPercentage = TotalPoints > 0 ? (TotalGrades / TotalPoints) * 100 : 0;

            // Determine letter grade based on the total percentage
            LetterGrade = GetLetterGrade(TotalPercentage);

            return Page();
        }

        private string GetLetterGrade(double percentage)
        {
            if (percentage >= 93) return "A";
            if (percentage >= 90) return "A-";
            if (percentage >= 87) return "B+";
            if (percentage >= 83) return "B";
            if (percentage >= 80) return "B-";
            if (percentage >= 77) return "C+";
            if (percentage >= 73) return "C";
            if (percentage >= 70) return "C-";
            if (percentage >= 67) return "D+";
            if (percentage >= 63) return "D";
            if (percentage >= 60) return "D-";
            return "F";
        }

    }

}
