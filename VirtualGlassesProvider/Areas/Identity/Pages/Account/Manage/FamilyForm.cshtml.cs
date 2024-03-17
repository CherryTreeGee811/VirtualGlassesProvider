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
    public class FamilyFormModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly GlassesStoreDbContext _context;


        public FamilyFormModel(
           UserManager<User> userManager,
           SignInManager<User> signInManager,
           GlassesStoreDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }


        [BindProperty(SupportsGet = true)]
        public InputModel Input { get; set; }


        public sealed class InputModel
        {
            [Display(Name = "First Name")]
            public string? FirstName { get; set; }


            [Display(Name = "Last Name")]
            public string? LastName { get; set; }


            [Display(Name = "Address")]
            public string? Address { get; set; }


            [Display(Name = "Email")]
            public string? Email { get; set; }


            [Display(Name = "Relationship")]
            public string? Relationship { get; set; }


            [Display(Name = "Phone Number")]
            public string? PhoneNumber { get; set; }


            public IFormFile? Image { get; set; }
        }


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            bool displayBlankMemberImage = true;

            if (id != null)
            {
                var member = await _context.FamilyMembers.Where(u => u.UserID == user.Id).FirstAsync(m => m.ID == id);
                if(member != null)
                {
                    if (!String.IsNullOrEmpty(member.FirstName))
                    {
                        Input.FirstName = member.FirstName;
                    }

                    if (!String.IsNullOrEmpty(member.LastName))
                    {
                        Input.LastName = member.LastName;
                    }
                    
                    if (!String.IsNullOrEmpty(member.Address))
                    {
                        Input.Address = member.Address;
                    }

                    if (!String.IsNullOrEmpty(member.Email))
                    {
                        Input.Email = member.Email;
                    }

                    if (!String.IsNullOrEmpty(member.Relationship))
                    {
                        Input.Relationship = member.Relationship;
                    }

                    if (!String.IsNullOrEmpty(member.PhoneNumber))
                    {
                        Input.PhoneNumber = member.PhoneNumber;
                    }

                    if (member.ImageID != null)
                    {
                        var uploadedImage = await _context.UploadedImages.FirstOrDefaultAsync(p => p.ID == member.ImageID);
                        if (uploadedImage != null)
                        {
                            ViewData["priorImage"] = $"data:image/jpg;base64,{Convert.ToBase64String(uploadedImage.Image)}";
                            ViewData["imageAlt"] = "Family Member Image";
                            displayBlankMemberImage = false;
                        }
                    }
                }
            }

            if(displayBlankMemberImage)
            {
                ViewData["priorImage"] = "\\images\\avatar.png";
                ViewData["imageAlt"] = "Placeholder Family Member Image";
            }
                
            return Page();
        }


        public async Task<IActionResult> OnPostAsync(IFormFile file, int? id)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage();
            }
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            UploadedImages image = null;

            if (file != null)
            {
                image = FileUploadService.ConvertFormFileToUploadedImageObject(file);
                _context.UploadedImages.Add(image);
                _context.SaveChanges();
            }

            FamilyMember familyMember = null;
            bool existingMember = false;
            if (id == null)
            {
               familyMember = new FamilyMember();
            } 
            else {
                familyMember = await _context.FamilyMembers.FindAsync(id);
               
                if(familyMember == null)
                {
                    familyMember = new FamilyMember();
                }
                else
                {
                    existingMember = true;
                    if(image == null)
                    {
                        image = await _context.UploadedImages.FindAsync(familyMember.ImageID);
                    }
                }
            }

            if (image != null)
            {
                familyMember.ImageID = image.ID;
            }
            
            familyMember.UserID = user.Id;
            familyMember.FirstName = Input.FirstName;
            familyMember.LastName = Input.LastName;
            familyMember.Address = Input.Address;
            familyMember.Email = Input.Email;
            familyMember.Relationship = Input.Relationship;
            familyMember.PhoneNumber = Input.PhoneNumber;
            if (existingMember) {
                _context.FamilyMembers.Update(familyMember);
            }
            else {
                _context.FamilyMembers.Add(familyMember);
            }
            _context.SaveChanges();
            return RedirectToPage("./Family");
        }
    }
}