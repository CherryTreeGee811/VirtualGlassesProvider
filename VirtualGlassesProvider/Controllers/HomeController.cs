using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using VirtualGlassesProvider.Models;
using VirtualGlassesProvider.Models.DTOs;

namespace VirtualGlassesProvider.Controllers
{
    public class HomeController : Controller
    {
        private readonly GlassesStoreDbContext _context;
        private const int PageSize = 10;

        public HomeController(GlassesStoreDbContext context)
        {
            _context = context;
        }

        public async Task<ActionResult> Index(int pageNumber = 1)
        {
            int excludeRecords = (pageNumber * PageSize) - PageSize;

            var glassQuery = _context.Glasses.Select(glass => new GlassDTO
            {
                ID = glass.glassesID,
                glassesBrandName = glass.glassesBrandName,
                Description = glass.Description,
                Price = glass.Price,
                color = glass.color,
                Style = glass.Style,
                Image = glass.Image
            });

            var model = await glassQuery.Skip(excludeRecords).Take(PageSize).ToListAsync();

            int totalRecords = await _context.Glasses.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalRecords / PageSize);

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = pageNumber;

            return View(model);
        }

        public IActionResult Search(string searchString)
        {
            if (String.IsNullOrEmpty(searchString))
            {
                return RedirectToAction("Index");
            }

            var glassClientList = new List<GlassDTO>();

            // Start with all glasses if the search string is not null or empty.
            var glassesQuery = _context.Glasses.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                // Modify the query to filter by any of the conditions.
                glassesQuery = glassesQuery.Where(g =>
                    g.glassesBrandName.Contains(searchString) ||
                    g.Style.Contains(searchString) ||
                    g.color.Contains(searchString) ||
                    g.Price.ToString().Contains(searchString));
            }

            // Project the filtered glasses to GlassDTO objects.
            foreach (Glasses glass in glassesQuery)
            {
                var glassClient = new GlassDTO()
                {
                    ID = glass.glassesID,
                    glassesBrandName = glass.glassesBrandName,
                    Description = glass.Description,
                    Price = glass.Price,
                    color = glass.color,
                    Style = glass.Style,
                    Image = glass.Image
                };

                glassClientList.Add(glassClient);
            }

            return View(glassClientList);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
