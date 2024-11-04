using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Models;
using RazorPagesMovie.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Client;
using System.Configuration;


namespace RazorPagesMovie.Pages.Users
{
    public class ChangeProfilePicModel : PageModel
    {
        private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;
        private readonly IWebHostEnvironment _environment;


        public ChangeProfilePicModel(IWebHostEnvironment environment, RazorPagesMovie.Data.RazorPagesMovieContext context)
        {
            _environment = environment;
            _context = context;

        }

        [BindProperty]
        public ImageUploadViewModel ViewModel { get; set; }
        public User CurrentUser { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
     
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) { return RedirectToPage("./Login"); }
            if (!int.TryParse(userIdClaim, out var userId)) { return RedirectToPage("./Login"); } // invalid userId

            //grab user object from Id passed to get
            var user = await _context.User.FirstOrDefaultAsync(m => m.Id == userId);
            if (user == null) { return NotFound(); }

            ViewModel = new ImageUploadViewModel()
            {
                Id = user.Id,
            };

            CurrentUser = user;
            return Page();
        }


        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            //grab current user using the id value passed to the View model
            var user = await _context.User.FirstOrDefaultAsync(m => m.Id == ViewModel.Id);
            string serverFolder;

            //user.FirstName + user.LastName + user.Id.ToString()
            if (!ModelState.IsValid || user == null || ViewModel.ImageFile == null)
            {
                return Page();
            }

            //Get file extension to append onto end of file path
            var ext = System.IO.Path.GetExtension(ViewModel.ImageFile.FileName);

            //if no user has no profile pic, create a new file path for the pic and save the path to the user model
            if (user.ProfilePic == null)
            {
                string dir = "Images/Profile_Pics/";
                if (user.IsInstructor) dir += "Instructors/";
                else dir += "Students/";
                dir += user.FirstName + user.LastName + user.Id.ToString() + ext;
                //append directory path onto root to get full path
                serverFolder = Path.Combine(_environment.WebRootPath, dir);
                user.ProfilePic = dir;
                await _context.SaveChangesAsync();
            }
            else
            {
                serverFolder = Path.Combine(_environment.WebRootPath, user.ProfilePic);
            }
            
            await ViewModel.ImageFile.CopyToAsync(new FileStream(serverFolder, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite));
            return RedirectToPage("./Profile");
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}

