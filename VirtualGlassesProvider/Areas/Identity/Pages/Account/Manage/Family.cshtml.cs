using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VirtualGlassesProvider.Models;
using VirtualGlassesProvider.Models.DataAccess;


namespace VirtualGlassesProvider.Areas.Identity.Pages.Account.Manage
{
    public class FamilyModel(
          UserManager<User> userManager,
          GlassesStoreDbContext context) : PageModel
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly GlassesStoreDbContext _context = context;


        [BindProperty(SupportsGet = true)]
        public ICollection<FamilyMember>? FamilyMembers { get; set; }


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

            FamilyMembers = _context.FamilyMembers
                .Where(u => u.UserID == user.Id)
                .ToHashSet();

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
