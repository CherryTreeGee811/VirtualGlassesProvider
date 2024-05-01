using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using VirtualGlassesProvider.Models;
using VirtualGlassesProvider.Models.DataAccess;


namespace VirtualGlassesProvider.Areas.Identity.Pages.Account.Manage
{
    public class FamilyModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly GlassesStoreDbContext _context;

        public FamilyModel(
          UserManager<User> userManager,
          SignInManager<User> signInManager,
          GlassesStoreDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }


        [BindProperty(SupportsGet = true)]
        public List<FamilyMember> familyMembers { get; set; }


        public async Task<IActionResult> OnGet(bool? error)
        {
            if(error == true)
            {
                ViewData["ErrorMessage"] = "Did not save family member, due to invalid information";
            }
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            familyMembers = _context.FamilyMembers.Where(u => u.UserID == user.Id).ToList();

            return Page();
        }


        public async Task<IActionResult> OnPostDeleteMemberAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var member = await _context.FamilyMembers.FindAsync(id);

            if (member != null)
            {
                _context.FamilyMembers.Remove(member);
                _context.SaveChanges();
            }

            return RedirectToPage();
        }
    }
}
