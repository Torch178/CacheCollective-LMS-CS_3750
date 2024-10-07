using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using RazorPagesMovie.Models;
using System.Configuration;
using System.Security.Claims;

namespace RazorPagesMovie.Pages.Users
{
    
    public class IndexModel : PageModel
    {
        private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;

        public IndexModel(RazorPagesMovie.Data.RazorPagesMovieContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User CurrentUser { get; set; } = default!;
        [BindProperty]
        public IList<Models.Course> Course { get; set; }
        [BindProperty]
        public IList<Models.Assignment> ToDoList { get; set; } = new List<Models.Assignment>();
        IEnumerable<Models.Assignment> SortedToDo { get; set; } = new List<Models.Assignment>();

        public async Task<IActionResult> OnGetAsync()
        {
            // Fetch user from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) { return RedirectToPage("./Login"); }
            if (!int.TryParse(userIdClaim, out var userId)) { return RedirectToPage("./Login"); } // invalid userId

            var user = await _context.User.FirstOrDefaultAsync(m => m.Id == userId);
            if (user == null) { return NotFound(); }

            //Get Course Content for user
            CurrentUser = user;
            if (CurrentUser.IsInstructor == false)
            {
                Course = await _context.Enrollment.Where(e => e.UserId == user.Id).Join(_context.Course, enrollment => enrollment.CourseId, course => course.CourseId, (enrollment, course) => course).ToListAsync();
            }
            else
            {
                Course = await _context.Course.Where(c => c.InstructorCourseId == user.Id).ToListAsync();
            }

            //Collect assignments from courses
            foreach (var course in Course)
            {
                IList<Assignment> Assignments = await _context.Assignment.Where(e => e.CourseId == course.CourseId).ToListAsync();
                foreach (var assignment in Assignments)
                {
                    ToDoList.Add(assignment);
                }
            }

            //Sort To-Do List by Due Date
            SortedToDo = ToDoList.OrderBy(q=>q.DueDate);
            ToDoList = SortedToDo.ToList();

            return Page();

            
        }

       
    }
}
