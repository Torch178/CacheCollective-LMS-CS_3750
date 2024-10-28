using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;

namespace RazorPagesMovie.Pages.Course.Assignment
{
    public class DetailsModel : PageModel
    {
        private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;

        public DetailsModel(RazorPagesMovie.Data.RazorPagesMovieContext context)
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
        public IList<Submission> Submissions { get; set; } = default!;
        public Models.Submission CurrentSubmission { get; set; }
        public RazorPagesMovie.Models.Assignment Assignment { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignment = await _context.Assignment.FirstOrDefaultAsync(m => m.Id == id);
            if (assignment == null)
            {
                return NotFound();
            }

            Assignment = assignment;

            // Fetch user from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) { return RedirectToPage("/Users/Login"); }
            if (!int.TryParse(userIdClaim, out var userId)) { return RedirectToPage("/Users/Login"); } // invalid userId

            var user = await _context.User.FirstOrDefaultAsync(m => m.Id == userId);
            if (user == null) { return NotFound(); }

            // Fetch the submissions for the specified assignmentId
            Submissions = await _context.Submission
                .Where(s => s.AssignmentId == id)
                .ToListAsync();

            CurrentSubmission = await _context.Submission.FirstOrDefaultAsync(s => s.AssignmentId == id && s.UserId == user.Id); // DOES NOT WORK UNTIL THE CORRECT USER ID IS STORED FOR EACH SUBMISSION (currently stores it under userId 0)

            AGrades = BGrades = CGrades = DGrades = FGrades = 0;
            foreach (var submission in Submissions)
            {
                if (submission.GradedPoints.HasValue == false) continue;

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

            return Page();
        }
    }
}