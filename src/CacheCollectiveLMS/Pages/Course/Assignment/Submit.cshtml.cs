using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;
using System.IO;
using Microsoft.EntityFrameworkCore;

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
        public Submission Submission { get; set; } = default!;

        [BindProperty]
        public string SubmittedText { get; set; } = string.Empty;

        [BindProperty]
        public IFormFile? SubmissionFile { get; set; }

        // Assignment property is not bound, it will be fetched manually.
        public RazorPagesMovie.Models.Assignment? Assignment { get; set; }

        private void PrintModelStateErrors()
        {
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    Console.WriteLine($"ModelState Error in {state.Key}: {error.ErrorMessage}");
                }
            }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (AssignmentId == 0)
            {
                return NotFound();
            }

            // Fetch the assignment to ensure Assignment is not null
            Assignment = await _context.Assignment.FirstOrDefaultAsync(a => a.Id == AssignmentId);
            if (Assignment == null)
            {
                return NotFound();
            }

            // Initialize the Submission object
            Submission = new Submission
            {
                AssignmentId = AssignmentId
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Fetch the assignment manually based on AssignmentId
            Assignment = await _context.Assignment.FirstOrDefaultAsync(a => a.Id == AssignmentId);
            if (Assignment == null)
            {
                return NotFound();
            }

            // Conditionally remove the validation for SubmittedText if the submission type is file upload
            if (Assignment.SubmissionType == SubmissionType.FileUpload)
            {
                ModelState.Remove("SubmittedText");
            }

            // Validate only Submission and necessary fields, not the Assignment property
            if (!ModelState.IsValid)
            {
                // Print model state errors to debug
                PrintModelStateErrors();
                return Page();
            }

            // Fetch or initialize the existing submission for this student
            var existingSubmission = await _context.Submission
                .FirstOrDefaultAsync(s => s.AssignmentId == AssignmentId && s.UserId == Submission.UserId);

            if (Assignment.SubmissionType == SubmissionType.TextEntry)
            {
                if (string.IsNullOrWhiteSpace(SubmittedText))
                {
                    ModelState.AddModelError(string.Empty, "Text submission cannot be empty.");
                    return Page();
                }

                if (existingSubmission != null)
                {
                    // Update the existing text submission
                    existingSubmission.SubmittedText = SubmittedText;
                    existingSubmission.SubmissionDate = DateTime.Now;
                }
                else
                {
                    // Create a new text submission
                    Submission.SubmittedText = SubmittedText;
                    Submission.SubmissionType = SubmissionType.TextEntry;
                    Submission.SubmissionDate = DateTime.Now;
                    Submission.GradedPoints = null;

                    _context.Submission.Add(Submission);
                }
            }
            else if (Assignment.SubmissionType == SubmissionType.FileUpload)
            {
                if (SubmissionFile == null)
                {
                    ModelState.AddModelError(string.Empty, "A file must be uploaded for this submission.");
                    return Page();
                }

                // Set up the directory to save the file
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Submissions");

                // Ensure the directory exists
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generate a unique file path
                var uniqueFileName = $"{Guid.NewGuid()}_{SubmissionFile.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                try
                {
                    // Delete old file if this is an update
                    if (existingSubmission != null && !string.IsNullOrEmpty(existingSubmission.FilePath))
                    {
                        var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingSubmission.FilePath);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    // Save the new file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await SubmissionFile.CopyToAsync(stream);
                    }

                    if (existingSubmission != null)
                    {
                        // Update the existing submission with new file details
                        existingSubmission.FilePath = $"Submissions/{uniqueFileName}";
                        existingSubmission.FileName = SubmissionFile.FileName;
                        existingSubmission.SubmissionDate = DateTime.Now;
                    }
                    else
                    {
                        // Create a new file submission
                        Submission.FilePath = $"Submissions/{uniqueFileName}";
                        Submission.FileName = SubmissionFile.FileName;
                        Submission.SubmissionType = SubmissionType.FileUpload;
                        Submission.SubmissionDate = DateTime.Now;
                        Submission.GradedPoints = null;

                        _context.Submission.Add(Submission);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"An error occurred while uploading the file: {ex.Message}");
                    return Page();
                }
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Redirect to the index page for the course's assignments
            return RedirectToPage("./Index", new { CourseId = this.CourseId });
        }
    }
}
