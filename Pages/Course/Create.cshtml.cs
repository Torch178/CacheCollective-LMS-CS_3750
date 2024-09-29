using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;

namespace RazorPagesMovie.Pages.Course
{
    public class CourseCreationModel : PageModel
    {
        private readonly RazorPagesMovieContext _context;

        public CourseCreationModel(RazorPagesMovieContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RazorPagesMovie.Models.Course CurrentCourse { get; set; } = default!;

        [BindProperty]
        public List<string> SelectedMeetingDays { get; set; } = new List<string>();

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("CurrentCourse.MeetingDays");

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }

                return Page();

            }

            if (SelectedMeetingDays == null || !SelectedMeetingDays.Any())
            {
                ModelState.AddModelError("SelectedMeetingDays", "Please select at least one meeting day.");
                return Page();
            }

            // Fetch user from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) { return RedirectToPage("./Login"); }
            if (!int.TryParse(userIdClaim, out var userId)) { return RedirectToPage("./Login"); } // invalid userId

            var loggedInUser = await _context.User.FirstOrDefaultAsync(m => m.Id == userId);
            if (loggedInUser == null) { return NotFound(); }

            if (loggedInUser.IsInstructor)
            {
                CurrentCourse.Instructor = $"{loggedInUser.FirstName} {loggedInUser.LastName}";
                CurrentCourse.InstructorCourseId = loggedInUser.Id;
            } else
            {
                ModelState.AddModelError(string.Empty, "Only instructors can create courses.");
                return Page();
            }

            CurrentCourse.MeetingDays = string.Join(", ", SelectedMeetingDays);
            _context.Course.Add(CurrentCourse);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }

}
