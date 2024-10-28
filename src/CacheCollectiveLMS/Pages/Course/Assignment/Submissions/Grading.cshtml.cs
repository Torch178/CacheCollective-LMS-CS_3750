using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;

namespace RazorPagesMovie.Pages.Course.Assignment.Submissions
{
    public class GradingModel : PageModel
    {
        private readonly RazorPagesMovieContext _context;

        public GradingModel(RazorPagesMovieContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Submission Submission { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? submissionId)
        {
            if (submissionId == null)
            {
                return NotFound();
            }

            Submission = await _context.Submission
                .FirstOrDefaultAsync(s => s.SubmissionId == submissionId);

            if (Submission == null)
            {
                return NotFound();
            }

            var assignment = await _context.Assignment
                .FirstOrDefaultAsync(a => a.Id == Submission.AssignmentId);

            if (assignment == null)
            {
                return NotFound();
            }

            ViewData["MaxPoints"] = assignment.MaxPoints;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int submissionId)
        {
            Submission = await _context.Submission.FindAsync(submissionId);

            if (Submission == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var gradedPoints = Request.Form["GradedPoints"];
                var instructorComments = Request.Form["InstructorComments"];

                if (double.TryParse(gradedPoints, out double points))
                {
                    Submission.GradedPoints = points;
                    Submission.InstructorComments = instructorComments;

                    _context.Entry(Submission).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    // Set TempData message
                    TempData["Message"] = "Grade saved successfully!";
                    return RedirectToPage("/Course/Assignment/Submissions/Submissions", new { assignmentId = Submission.AssignmentId });
                }
                else
                {
                    ModelState.AddModelError("GradedPoints", "Invalid points.");
                }
            }

            return Page();
        }
    }
}
