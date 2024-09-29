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
using RazorPagesMovie.ViewModels;

namespace RazorPagesMovie.Pages.Users
{
    public class Edit_ProfileModel : PageModel
    {
        private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;

        //grab database context
        public Edit_ProfileModel(RazorPagesMovie.Data.RazorPagesMovieContext context)
        {
            _context = context;
        }

        //create view model for changing user profile
        [BindProperty]
        public EditProfileViewModel ViewModel { get; set; }

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

            //Use View Model to grab specific data we allow the user to update
            ViewModel = new EditProfileViewModel()
            {
                //Id is un-editable, just here to map view model to the correct user
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Birthdate = user.Birthdate,
            };
            

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            //grab current user using the id value passed to the EditProfile View model
            var user = await _context.User.FirstOrDefaultAsync(m => m.Id == ViewModel.Id);
            
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (user != null)
            {
                //update user model with modified data from view model form
                user.Email = ViewModel.Email;
                user.FirstName = ViewModel.FirstName;
                user.LastName = ViewModel.LastName;
                user.Birthdate = ViewModel.Birthdate;
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
