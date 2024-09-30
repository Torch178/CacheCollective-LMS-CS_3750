using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;
using RazorPagesMovie.ViewModels;

namespace RazorPagesMovie.Pages.Users
{
    public class ChangeProfilePicModel : PageModel
    {
        private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;
        private readonly IWebHostEnvironment _environment;

        public ChangeProfilePicModel(IWebHostEnvironment environment,RazorPagesMovie.Data.RazorPagesMovieContext context)
        {
            _environment = environment;
            _context = context;
        }

        [BindProperty]
        public ImageUploadViewModel ViewModel { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //grab user object from Id passed to get
            var user = await _context.User.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            ViewModel = new ImageUploadViewModel()
            {
                Id = user.Id,
                ImageFile = null
            };


            return Page();
        }

        [BindProperty]
        public User User { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            //grab current user using the id value passed to the View model
            var user = await _context.User.FirstOrDefaultAsync(m => m.Id == ViewModel.Id);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (user != null)
            {
                if (ViewModel.ImageFile != null) {

                    string dir = "Images/Profile_Pics/";
                    if (user.IsInstructor) dir += "Instructors/";
                    else dir += "Students/";
                    dir += user.FirstName + "_" + user.LastName + Guid.NewGuid().ToString() + ViewModel.ImageFile.FileName;

                    //append directory path onto root to get full path
                    string serverFolder = Path.Combine(_environment.WebRootPath, dir);

                    //store newly uploaded file to path in serverFolder
                    await ViewModel.ImageFile.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
                    //save path and file name of image to user model
                    user.ProfilePic = serverFolder;
                }
                //update database entity
             
                _context.Attach(user).State = EntityState.Modified;
            }

            try
            {
                //save changes to database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(ViewModel.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Profile");
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}
    
