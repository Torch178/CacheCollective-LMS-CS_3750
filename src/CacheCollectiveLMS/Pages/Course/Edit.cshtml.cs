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

namespace RazorPagesMovie.Pages.Course
{
    public class EditModel : PageModel
    {
        private readonly RazorPagesMovieContext _context;

        public EditModel(RazorPagesMovieContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RazorPagesMovie.Models.Course CurrentCourse { get; set; } = default!;

        [BindProperty]
        public List<string> SelectedMeetingDays { get; set; } = new List<string>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CurrentCourse = await _context.Course.FirstOrDefaultAsync(m => m.CourseId == id);

            if (CurrentCourse == null)
            {
                return NotFound();
            }

            // Populate SelectedMeetingDays from the comma-separated string in CurrentCourse.MeetingDays
            if (!string.IsNullOrEmpty(CurrentCourse.MeetingDays))
            {
                SelectedMeetingDays = CurrentCourse.MeetingDays.Split(", ").ToList();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("CurrentCourse.MeetingDays");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (SelectedMeetingDays == null || !SelectedMeetingDays.Any())
            {
                ModelState.AddModelError("SelectedMeetingDays", "Please select at least one meeting day.");
                return Page();
            }

            CurrentCourse.MeetingDays = string.Join(", ", SelectedMeetingDays);
            _context.Attach(CurrentCourse).State = EntityState.Modified;

            // Fetch user from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) { return RedirectToPage("./Login"); }
            if (!int.TryParse(userIdClaim, out var userId)) { return RedirectToPage("./Login"); } // invalid userId

            var loggedInUser = await _context.User.FirstOrDefaultAsync(m => m.Id == userId);
            if (loggedInUser == null) { return NotFound(); }

            if (loggedInUser == null)
            {
                return Unauthorized();
            }

            if (loggedInUser.IsInstructor)
            {
                CurrentCourse.Instructor = $"{loggedInUser.FirstName} {loggedInUser.LastName}";
                CurrentCourse.InstructorCourseId = loggedInUser.Id;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Only instructors can create courses.");
                return Page();
            }


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(CurrentCourse.CourseId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool CourseExists(int id)
        {
            return _context.Course.Any(e => e.CourseId == id);
        }
    }
}
