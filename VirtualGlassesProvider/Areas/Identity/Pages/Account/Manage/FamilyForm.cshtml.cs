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
    public class FamilyFormModel(
        UserManager<User> userManager,
        GlassesStoreDbContext context) : PageModel
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly GlassesStoreDbContext _context = context;


        [BindProperty(SupportsGet = true)]
        public InputModel Input { get; set; } = new InputModel();


        public sealed class InputModel
        {
            [Display(Name = "First Name")]
            [RegularExpression(@"^[a-zA-Z\s]{1,30}$")]
            public string? FirstName { get; set; }


            [Display(Name = "Last Name")]
            [RegularExpression(@"^[a-zA-Z\s]{1,30}$")]
            public string? LastName { get; set; }


            [Display(Name = "Address")]
            [RegularExpression(@"^[0-9]{1,6}\s{1}[a-zA-Z]{1,20}\s?[a-zA-Z0-9\s\.\,]{0,30}$")]
            public string? Address { get; set; }


            [Display(Name = "Email")]
            [RegularExpression(@"^[0-9a-zA-Z]{1,30}@{1}[0-9a-zA-Z]{1,30}\.{1}[0-9a-zA-Z]{1,30}\.?[0-9a-zA-Z]{0,30}$")]
            public string? Email { get; set; }


            [Display(Name = "Relationship")]
            [RegularExpression(@"^[0-9a-zA-Z\s]{1,40}$")]
            public string? Relationship { get; set; }


            [Display(Name = "Phone Number")]
            [RegularExpression(@"^\(?[0-9]{3}\)?[\s\-]?[0-9]{3}[\s\-]?[0-9]{4}$")]
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
                    if (!string.IsNullOrEmpty(member.FirstName))
                    {
                        Input.FirstName = member.FirstName;
                    }

                    if (!string.IsNullOrEmpty(member.LastName))
                    {
                        Input.LastName = member.LastName;
                    }
                    
                    if (!string.IsNullOrEmpty(member.Address))
                    {
                        Input.Address = member.Address;
                    }

                    if (!string.IsNullOrEmpty(member.Email))
                    {
                        Input.Email = member.Email;
                    }

                    if (!string.IsNullOrEmpty(member.Relationship))
                    {
                        Input.Relationship = member.Relationship;
                    }

                    if (!string.IsNullOrEmpty(member.PhoneNumber))
                    {
                        Input.PhoneNumber = member.PhoneNumber;
                    }

                    if (member.ImageID != null)
                    {
                        var uploadedImage = await _context.UploadedImages.FirstOrDefaultAsync(p => p.ID == member.ImageID);
                        if (uploadedImage?.Image != null)
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
            if(id == null)
            {
                // New Record if null
                if(!ModelState.IsValid)
                {
                    return RedirectToPage("./Family", new { error = true });
                }
            }

            // ToDo: Add more checks
            // ToDo: Send status messages to family page

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            UploadedImages? image = null;

            if (file != null)
            {
                image = FileUploadService.ConvertFormFileToUploadedImageObject(file);
                if (image != null)
                {
                    _context.UploadedImages.Add(image);
                    _context.SaveChanges();
                }
            }

            FamilyMember? familyMember;
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

                    image ??= await _context.UploadedImages.FindAsync(familyMember.ImageID);
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