using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;

namespace RazorPagesMovie.Pages.Course.Assignment
{
    public class SubmitModel : PageModel
    {
        private readonly RazorPagesMovieContext _context;

        public SubmitModel(RazorPagesMovieContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int AssignmentId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CourseId { get; set; }


        [BindProperty]
        public string SubmittedText { get; set; } = string.Empty;

        [BindProperty]
        public IFormFile? SubmissionFile { get; set; }

        public RazorPagesMovie.Models.Assignment Assignment { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            if (AssignmentId == 0)
            {
                return NotFound();
            }

            Assignment = await _context.Assignment.FirstOrDefaultAsync(a => a.Id == AssignmentId);
            if (Assignment == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var assignment = await _context.Assignment.FirstOrDefaultAsync(a => a.Id == AssignmentId);
            if (assignment == null)
            {
                return NotFound();
            }

            Assignment = assignment; // Assign fetched assignment to the Assignment property

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
            {
                return RedirectToPage("/Login");
            }

            var submission = new Submission
            {
                AssignmentId = AssignmentId,
                UserId = userId,
                SubmissionDate = DateTime.Now
            };

            if (assignment.SubmissionType == SubmissionType.TextEntry && !string.IsNullOrWhiteSpace(SubmittedText))
            {
                submission.SubmittedText = SubmittedText;
            }
            else if (assignment.SubmissionType == SubmissionType.FileUpload && SubmissionFile != null)
            {
                var filePath = $"Submissions/{Guid.NewGuid()}_{SubmissionFile.FileName}";
                using (var stream = System.IO.File.Create(filePath))
                {
                    await SubmissionFile.CopyToAsync(stream);
                }
                submission.FilePath = filePath;
                submission.FileName = SubmissionFile.FileName;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Please provide a valid submission based on the assignment type.");
                return Page();
            }

            _context.Submission.Add(submission);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index", new { courseId = Assignment.CourseId });
        }

    }
}
