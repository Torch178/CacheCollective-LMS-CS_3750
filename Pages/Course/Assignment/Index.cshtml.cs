using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;

namespace RazorPagesMovie.Pages.Course.Assignment
{
    public class IndexModel : PageModel
    {
        private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;

        public IndexModel(RazorPagesMovie.Data.RazorPagesMovieContext context)
        {
            _context = context;
        }

        public IList<RazorPagesMovie.Models.Assignment> Assignment { get;set; } = new List<Models.Assignment>();

        // Use the CourseID passed as a query parameter
        [BindProperty(SupportsGet = true)]
        public int CourseId { get; set; }

        public string CourseName { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Fetch the course by CourseId to get its name
            var course = await _context.Course.FirstOrDefaultAsync(c => c.CourseId == CourseId);
            if (course == null)
            {
                return NotFound();
            }

            CourseName = course.Title;

            Assignment = await _context.Assignment
                .Where(a => a.CourseId == CourseId)
                .ToListAsync();

            return Page();
        }
    }
}
