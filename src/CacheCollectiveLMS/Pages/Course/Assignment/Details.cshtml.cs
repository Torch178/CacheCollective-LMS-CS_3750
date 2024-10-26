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
    public class DetailsModel : PageModel
    {
        private readonly RazorPagesMovieContext _context;

        public DetailsModel(RazorPagesMovieContext context)
        {
            _context = context;
        }

        public RazorPagesMovie.Models.Assignment Assignment { get; set; } = default!;
        public List<Submission> Submissions { get; set; } = new List<Submission>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignment = await _context.Assignment.FirstOrDefaultAsync(m => m.Id == id);
            if (assignment == null)
            {
                return NotFound();
            }

            Assignment = assignment;

            // Fetch submissions related to the assignment
            Submissions = await _context.Submission
                .Where(s => s.AssignmentId == Assignment.Id)
                .ToListAsync();

            return Page();
        }
    }
}
