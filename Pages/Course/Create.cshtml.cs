using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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
                return Page();
            }

            if (SelectedMeetingDays == null || !SelectedMeetingDays.Any())
            {
                ModelState.AddModelError("SelectedMeetingDays", "Please select at least one meeting day.");
                return Page();
            }

            CurrentCourse.MeetingDays = string.Join(", ", SelectedMeetingDays);
            _context.Course.Add(CurrentCourse);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }




    }

}
