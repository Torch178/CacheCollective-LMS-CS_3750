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

namespace RazorPagesMovie.Pages.Course.Assignment
{
    public class IndexModel : PageModel
    {
        private readonly RazorPagesMovieContext _context;

        public IndexModel(RazorPagesMovieContext context)
        {
            _context = context;
        }

        public IList<RazorPagesMovie.Models.Assignment> Assignment { get;set; } = new List<Models.Assignment>();

        // A dictionary to store each assignment's grade
        public Dictionary<int, double?> AssignmentGrades { get; set; } = new Dictionary<int, double?>();

        [BindProperty(SupportsGet = true)]
        public int CourseId { get; set; }

        public string CourseName { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var course = await _context.Course.FirstOrDefaultAsync(c => c.CourseId == CourseId);
            if (course == null)
            {
                return NotFound();
            }

            CourseName = course.Title;

            Assignment = await _context.Assignment
                .Where(a => a.CourseId == CourseId)
                .ToListAsync();

            // Retrieve the userId from the claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) { return RedirectToPage("./Login"); }

            if (!int.TryParse(userIdClaim, out var userId)) { return RedirectToPage("./Login"); }

            // Fetch all submissions for the assignments in this course for the current user
            var submissions = await _context.Submission
                .Where(s => s.UserId == userId && s.Assignment.CourseId == CourseId)
                .ToListAsync();

            // Populate the dictionary with assignment IDs and their grades
            foreach (var assignment in Assignment)
            {
                var submission = submissions.FirstOrDefault(s => s.AssignmentId == assignment.Id);
                AssignmentGrades[assignment.Id] = submission?.GradedPoints; // null if not graded
            }

            return Page();
        }
    }
}

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using Microsoft.EntityFrameworkCore;
//using RazorPagesMovie.Data;
//using RazorPagesMovie.Models;

//namespace RazorPagesMovie.Pages.Course.Assignment
//{
//    public class IndexModel : PageModel
//    {
//        private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;

//        public IndexModel(RazorPagesMovie.Data.RazorPagesMovieContext context)
//        {
//            _context = context;
//        }

//        public IList<RazorPagesMovie.Models.Assignment> Assignment { get;set; } = new List<Models.Assignment>();

//        // Use the CourseID passed as a query parameter
//        [BindProperty(SupportsGet = true)]
//        public int CourseId { get; set; }

//        public string CourseName { get; set; }

//        public async Task<IActionResult> OnGetAsync()
//        {
//            // Fetch the course by CourseId to get its name
//            var course = await _context.Course.FirstOrDefaultAsync(c => c.CourseId == CourseId);
//            if (course == null)
//            {
//                return NotFound();
//            }

//            CourseName = course.Title;

//            Assignment = await _context.Assignment
//                .Where(a => a.CourseId == CourseId)
//                .ToListAsync();

//            return Page();
//        }
//    }
//}
