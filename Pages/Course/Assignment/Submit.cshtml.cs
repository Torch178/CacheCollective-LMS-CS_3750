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

        public RazorPagesMovie.Models.Assignment Assignment { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            if (AssignmentId == 0)
            {
                return NotFound();
            }

            // Fetch the assignment to ensure Model.Assignment is not null
            Assignment = await _context.Assignment.FirstOrDefaultAsync(a => a.Id == AssignmentId);
            if (Assignment == null)
            {
                return NotFound();
            }

            // Ensure the Submission property is also initialized
            Submission = new Submission
            {
                AssignmentId = AssignmentId
            };

            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Fetch the assignment to determine submission type
            Assignment = await _context.Assignment.FindAsync(Submission.AssignmentId);
            if (Assignment == null)
            {
                return NotFound();
            }

            // Validate based on the SubmissionType
            if (Assignment.SubmissionType == SubmissionType.TextEntry)
            {
                if (string.IsNullOrWhiteSpace(SubmittedText))
                {
                    ModelState.AddModelError(string.Empty, "Text submission cannot be empty.");
                    return Page();
                }

                Submission.SubmittedText = SubmittedText;
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

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await SubmissionFile.CopyToAsync(stream);
                }

                // Set file properties in Submission model
                Submission.FilePath = Path.Combine("Submissions", uniqueFileName); // relative path for serving the file
                Submission.FileName = SubmissionFile.FileName;
            }

            Submission.SubmissionDate = DateTime.Now;
            _context.Submission.Add(Submission);
            await _context.SaveChangesAsync();

            // Redirect to the index page for the course's assignments
            return RedirectToPage("./Index", new { CourseId = this.CourseId });
        }

    }
}
