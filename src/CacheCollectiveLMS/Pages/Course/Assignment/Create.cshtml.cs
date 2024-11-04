using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;
using RazorPagesMovie.Services;

namespace RazorPagesMovie.Pages.Course.Assignment
{
    public class CreateModel : PageModel
    {
        private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;
        private readonly NotificationService _notificationService;

        public CreateModel(RazorPagesMovie.Data.RazorPagesMovieContext context, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }


        // Use the CourseID passed as a query parameter
        [BindProperty(SupportsGet = true)]
        public int CourseId { get; set; }

        public async Task<IActionResult> OnGet()
        {
            Assignment = new RazorPagesMovie.Models.Assignment
            {
                DueDate = DateTime.Now
            };
            var course = await _context.Course.FirstOrDefaultAsync(c => c.CourseId == CourseId);
            if (course == null)
            {
                return NotFound();
            }

            return Page();
        }

        [BindProperty]
        public RazorPagesMovie.Models.Assignment Assignment { get; set; } = default!;


        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Assignment.CourseId = CourseId;
            _context.Assignment.Add(Assignment);
            await _context.SaveChangesAsync();

            var students = await _context.Enrollment.Where(e => e.CourseId == CourseId).ToListAsync();
            foreach (var student in students)
            {
                await _notificationService.AddNotificationAsync(student.UserId, CourseId, Assignment.Id, "Created");
            }

            return RedirectToPage("Index", new { courseId = CourseId });
        }
    }
}
