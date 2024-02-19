// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using VirtualGlassesProvider.Models;
using VirtualGlassesProvider.Models.DataAccess;
using VirtualGlassesProvider.Services;


namespace VirtualGlassesProvider.Areas.Identity.Pages.Account.Manage
{
    public class ProfileModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly GlassesStoreDbContext _context;

        public ProfileModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            GlassesStoreDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }


        [BindProperty(SupportsGet = true)]
        public InputModel Input { get; set; }


        public sealed class InputModel
        {
            public string DisplayName { get; set; }

            public IFormFile? Image { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserID == user.Id);

            if(profile == null)
            {
                profile = new Profiles();
                profile.UserID = user.Id;
                _context.Add(profile);
                await _context.SaveChangesAsync();
            }
            else
            {
                Input.DisplayName = profile.DisplayName;
                var uploadedImage = await _context.UploadedImages.FirstOrDefaultAsync(p => p.ID == profile.ImageID);
                ViewData["priorImage"] = Convert.ToBase64String(uploadedImage.Image);
            }
            
            return Page();
        }


        public async Task<IActionResult> OnPostAsync(IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
           
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserID == user.Id);

            UploadedImages image = null;

            if (file != null)
            {
                image = FileUploadService.ConvertFormFileToUploadedImageObject(file);

                if (profile.ImageID != null)
                {
                    _context.UploadedImages.Update(image);
                }
                else
                {
                    _context.UploadedImages.Add(image);
                }
                _context.SaveChanges();
            }

            profile.ImageID = image.ID;
            profile.UserID = user.Id;
            profile.DisplayName = Input.DisplayName;

            _context.Profiles.Update(profile);
            _context.SaveChanges();

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
