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

            // Fetch the submission with the given submissionId
            Submission = await _context.Submission
                .FirstOrDefaultAsync(s => s.SubmissionId == submissionId);

            if (Submission == null)
            {
                return NotFound();
            }

            // Fetch the assignment for max points
            var assignment = await _context.Assignment
                .FirstOrDefaultAsync(a => a.Id == Submission.AssignmentId);

            if (assignment == null)
            {
                return NotFound();
            }

            ViewData["MaxPoints"] = assignment.MaxPoints;

            return Page();
        }

        // Method to handle the form submission
        public async Task<IActionResult> OnPostAsync(int submissionId)
        {
            // Fetch the submission
            Submission = await _context.Submission.FindAsync(submissionId);

            if (Submission == null)
            {
                return NotFound();
            }

            // Update the GradedPoints and Comments
            if (ModelState.IsValid)
            {
                var gradedPoints = Request.Form["GradedPoints"];
                var instructorComments = Request.Form["InstructorComments"];

                if (double.TryParse(gradedPoints, out double points))
                {
                    // Update the graded points
                    Submission.GradedPoints = points;

                    // Update the instructor comments
                    Submission.InstructorComments = instructorComments;

                    // Mark the entity as modified
                    _context.Entry(Submission).State = EntityState.Modified;

                    // Save the changes to the database
                    await _context.SaveChangesAsync();

                    TempData["Message"] = "Grade saved successfully!";
                    return RedirectToPage("Submissions", new { assignmentId = Submission.AssignmentId });
                }
                else
                {
                    ModelState.AddModelError("GradedPoints", "Invalid points.");
                }
            }

            // If validation fails, return the page again with the same data
            return Page();
        }
    }
}
