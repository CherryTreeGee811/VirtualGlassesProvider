using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VirtualGlassesProvider.Models;

namespace VirtualGlassesProvider.Controllers
{
    public class RoleController : Controller
    {
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<User> _userManager;

        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;

            _userManager = userManager;

        }

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
            if (_roleManager.RoleExistsAsync(role.Name).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(role.Name)).GetAwaiter().GetResult();
            }

            return RedirectToAction("Index");
        }



    }
}
