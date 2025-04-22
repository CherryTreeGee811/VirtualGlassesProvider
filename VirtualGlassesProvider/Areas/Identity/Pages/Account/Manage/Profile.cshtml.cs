// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using VirtualGlassesProvider.Models;
using VirtualGlassesProvider.Models.DataAccess;
using VirtualGlassesProvider.Services;


namespace VirtualGlassesProvider.Areas.Identity.Pages.Account.Manage
{
    public class ProfileModel(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        GlassesStoreDbContext context
        )
        : PageModel
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly SignInManager<User> _signInManager = signInManager;
        private readonly GlassesStoreDbContext _context = context;


        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; } = string.Empty;


        [BindProperty(SupportsGet = true)]
        public InputModel Input { get; set; } = new InputModel();


        public sealed class InputModel
        {
            [Display(Name = "First Name")]
            [RegularExpression(@"^[a-zA-Z\s]{1,30}$", ErrorMessage = "First name must be a-zA-Z and less than 30 characters")]
            public string FirstName { get; set; } = string.Empty;


            [Display(Name = "Last Name")]
            [RegularExpression(@"^[a-zA-Z\s]{1,30}$", ErrorMessage = "Last name must be a-zA-Z and less than 30 characters")]
            public string LastName { get; set; } = string.Empty;


            [Display(Name = "Address")]
            [RegularExpression(@"^[0-9]{1,6}\s{1}[a-zA-Z]{1,20}\s?[a-zA-Z0-9\s\.\,]{0,30}$", ErrorMessage = "Invalid address")]
            public string Address { get; set; } = string.Empty;


            [Display(Name = "Phone Number")]
            [RegularExpression(@"^\(?[0-9]{3}\)?[\s\-]?[0-9]{3}[\s\-]?[0-9]{4}$", ErrorMessage = "Phone number must follow format XXX-XXX-XXXX or XXX XXX XXXX")]
            public string PhoneNumber { get; set; } = string.Empty;


            [MaxLength(30, ErrorMessage = "Display name must be less than 30 Characters!")]
            public string DisplayName { get; set; } = string.Empty;


            public IFormFile? Image { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if(user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserID == user.Id);

            if(profile == null)
            {
                profile = new Profiles
                {
                    UserID = user.Id
                };
                _context.Add(profile);
                await _context.SaveChangesAsync();
            }

            if(profile.DisplayName != null)
            {
                Input.DisplayName = profile.DisplayName;
            }

            if (profile.FirstName != null)
            {
                Input.FirstName = profile.FirstName;
            }

            if (profile.LastName != null)
            {
                Input.LastName = profile.LastName;
            }

            if (profile.Address != null)
            {
                Input.Address = profile.Address;
            }

            if (profile.PhoneNumber != null)
            {
                Input.PhoneNumber = profile.PhoneNumber;
            }

            if (profile.ImageID != null)
            {
                var uploadedImage = await _context.UploadedImages.FirstOrDefaultAsync(p => p.ID == profile.ImageID);
                if (uploadedImage?.Image != null)
                {
                    ViewData["priorImage"] = $"data:image/jpg;base64,{Convert.ToBase64String(uploadedImage.Image)}";
                    ViewData["imageAlt"] = "Profile Image";
                }
            }
            else
            {
                ViewData["priorImage"] = "\\images\\avatar.png";
                ViewData["imageAlt"] = "Placeholder Profile Image";
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

            UploadedImages? image = null;

            if (file != null)
            {
                image = FileUploadService.ConvertFormFileToUploadedImageObject(file);
                if (image != null)
                {
                    if (profile?.ImageID != null)
                    {
                        _context.UploadedImages.Update(image);
                    }
                    else
                    {
                        _context.UploadedImages.Add(image);
                    }
                    _context.SaveChanges();
                }
            }
            if (profile != null)
            {
                if (image != null)
                {
                    profile.ImageID = image.ID;
                }
                profile.UserID = user.Id;
                profile.DisplayName = Input.DisplayName;
                profile.Address = Input.Address;
                profile.FirstName = Input.FirstName;
                profile.LastName = Input.LastName;
                profile.PhoneNumber = Input.PhoneNumber;
                _context.Profiles.Update(profile);
                _context.SaveChanges();
            }
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
