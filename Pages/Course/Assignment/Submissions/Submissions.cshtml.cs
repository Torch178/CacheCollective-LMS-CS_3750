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


        public IList<Submission> Submission { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? assignmentId)
        {
            if (assignmentId == null)
            {
                return NotFound();
            }

            // Fetch the assignment to get the MaxPoints
            var assignment = await _context.Assignment
                .FirstOrDefaultAsync(a => a.Id == assignmentId);

            if (assignment == null)
            {
                return NotFound();
            }

            // Fetch the submissions for the specified assignmentId
            Submission = await _context.Submission
                .Where(s => s.AssignmentId == assignmentId)
                .ToListAsync();

            foreach (var submission in Submission)
            {
                double gradePercentage = (submission.GradedPoints ?? 0) / assignment.MaxPoints;
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
