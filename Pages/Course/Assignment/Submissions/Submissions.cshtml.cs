using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;

namespace RazorPagesMovie.Pages.Course.Assignment.Submissions
{
    public class SubmissionsModel : PageModel
    {
        private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;

        public SubmissionsModel(RazorPagesMovie.Data.RazorPagesMovieContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int AGrades { get; set; }
        public int BGrades { get; set; }
        public int CGrades { get; set; }
        public int DGrades { get; set; }
        public int FGrades { get; set; }

        [BindProperty]
        public bool SubmissionsGraded { get; set; }

        [BindProperty]
        public IList<Submission> Submission { get; set; } = default!;
        public Models.Assignment CurrentAssignment { get; set; }

        public async Task<IActionResult> OnGetAsync(int? assignmentId)
        {
            AGrades = BGrades = CGrades = DGrades = FGrades = 0;

            if (assignmentId == null)
            {
                return NotFound();
            }

            // Fetch the assignment to get the MaxPoints
            var assignment = await _context.Assignment
                .FirstOrDefaultAsync(a => a.Id == assignmentId);

            CurrentAssignment = assignment;

            if (assignment == null)
            {
                return NotFound();
            }
            int assignmentsGraded = 0;

            // Fetch the submissions for the specified assignmentId
            Submission = await _context.Submission
                .Where(s => s.AssignmentId == assignmentId)
                .ToListAsync();

            foreach (var submission in Submission)
            {
                if (submission.GradedPoints.HasValue == false) continue;
                assignmentsGraded += 1;

                double gradePercentage = (submission.GradedPoints ?? 0) / assignment.MaxPoints * 100;
                if (gradePercentage >= 90)
                {
                    AGrades++;
                }
                else if (gradePercentage >= 80)
                {
                    BGrades++;
                }
                else if (gradePercentage >= 70)
                {
                    CGrades++;
                }
                else if (gradePercentage >= 60)
                {
                    DGrades++;
                }
                else
                {
                    FGrades++;
                }
            }

            if (assignmentsGraded >= Submission.Count)
            {
                SubmissionsGraded = true;
            }

            if (Submission == null || Submission.Count == 0)
            {
                ViewData["Message"] = "No submissions found for this assignment.";
            }
            else
            {
                // Add the MaxPoints information to ViewData so it can be accessed in the view
                ViewData["MaxPoints"] = assignment.MaxPoints;
            }

            return Page();
        }
    }
}
