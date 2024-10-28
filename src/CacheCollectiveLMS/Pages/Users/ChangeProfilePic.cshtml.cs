using System;
using System.IO;
using System.Collections;
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


        public ChangeProfilePicModel(IWebHostEnvironment environment, RazorPagesMovie.Data.RazorPagesMovieContext context)
        {
            _environment = environment;
            _context = context;

        }

        [BindProperty]
        public ImageUploadViewModel ViewModel { get; set; }
        public User User { get; set; }
        
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
            };

            User = user;
            return Page();
        }


        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            //grab current user using the id value passed to the View model
            var user = await _context.User.FirstOrDefaultAsync(m => m.Id == ViewModel.Id);
            //Store path for old profile pic to be deleted
            string? oldProfilePic = user.ProfilePic;

            if (!ModelState.IsValid)
            {
                return Page();
            }



            if (user != null)
            {
                if (ViewModel.ImageFile != null)
                {

                    string dir = "Images/Profile_Pics/";
                    if (user.IsInstructor) dir += "Instructors/";
                    else dir += "Students/";
                    dir += user.FirstName + "_" + user.LastName + Guid.NewGuid().ToString() + ViewModel.ImageFile.FileName;
                    //append directory path onto root to get full path
                    string serverFolder = Path.Combine(_environment.WebRootPath, dir);


                    //store newly uploaded file to path in serverFolder
                    await ViewModel.ImageFile.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

                    //save path and file name of image in local project directory to user model
                    user.ProfilePic = dir;

                }
                //update database entity

                _context.Attach(user).State = EntityState.Modified;
            }

            try
            {

                //save changes to database
                var changes = await _context.SaveChangesAsync();

                //*!---->Bug<----!*
                //*****************************Delete Old profile pic algorthim causing an error
                //delete old file after updating database with new file path
                //if (!string.IsNullOrWhiteSpace(oldProfilePic))
                //{
                //    //check to see that database has been udated before deleting old file
                //    while (!(changes > 0))
                //    {
                //        //Do Nothing
                //    }
                //    string path = Path.Combine(_environment.WebRootPath, oldProfilePic);
                //    FileInfo file = new FileInfo(path);


                //    if (file.Exists) { file.MoveTo(Path.Combine(_environment.WebRootPath, "Images/Profile_Pics/OldProfilePics")); }
                //}

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

