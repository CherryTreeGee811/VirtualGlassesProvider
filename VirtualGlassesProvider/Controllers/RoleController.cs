using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace VirtualGlassesProvider.Controllers
{
    public class RoleController(RoleManager<IdentityRole> roleManager) 
        : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;


        [HttpGet]
        public IActionResult Index()
        {
            var roles = _roleManager.Roles;

            return View(roles);
        }


        [HttpGet]
        public ViewResult CreateRole()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateRole(IdentityRole role)
        {
            if (role == null || string.IsNullOrWhiteSpace(role.Name))
            {
                ModelState.AddModelError(string.Empty, "Role name cannot be null or empty.");
                return View(role);
            }

            if (!await _roleManager.RoleExistsAsync(role.Name))
            {
                await _roleManager.CreateAsync(new IdentityRole(role.Name));
            }

            return RedirectToAction("Index");
        }
    }
}
