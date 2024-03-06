using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using VirtualGlassesProvider.Models;
using VirtualGlassesProvider.Models.DataAccess;
using VirtualGlassesProvider.Models.DTOs;
using Python.Runtime;
using VirtualGlassesProvider.CustomAttributes;
using Microsoft.AspNetCore.Authorization;
using System.Drawing;
using VirtualGlassesProvider.Services;
using VirtualGlassesProvider.Models.ViewModels;


namespace VirtualGlassesProvider.Controllers
{
    public sealed class HomeController : Controller
    {
        private readonly GlassesStoreDbContext _context;
        private const int PageSize = 10;
        private readonly UserManager<User> _userManager;


        public HomeController(GlassesStoreDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        [HttpGet]
        public async Task<ActionResult> Index(int pageNumber = 1)
        {
            int excludeRecords = (pageNumber * PageSize) - PageSize;

            var glassesQuery = _context.Glasses.Select(glasses => new GlassesDTO
            {
                ID = glasses.ID,
                BrandName = glasses.BrandName,
                Description = glasses.Description,
                Price = glasses.Price,
                Colour = glasses.Colour,
                Style = glasses.Style,
                Image = glasses.Image
            });

            var model = await glassesQuery.Skip(excludeRecords).Take(PageSize).ToListAsync();

            int totalRecords = await _context.Glasses.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalRecords / PageSize);

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = pageNumber;

            return View(model);
        }


        [HttpPost]
        public IActionResult Search(string searchString)
        {
            if (String.IsNullOrEmpty(searchString))
            {
                return RedirectToAction("Index");
            }

            var glassesClientList = new List<GlassesDTO>();

            // Start with all glasses if the search string is not null or empty.
            var glassesQuery = _context.Glasses.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                // Modify the query to filter by any of the conditions.
                glassesQuery = glassesQuery.Where(g =>
                    g.BrandName.Contains(searchString) ||
                    g.Style.Contains(searchString) ||
                    g.Colour.Contains(searchString) ||
                    g.Price.ToString().Contains(searchString));
            }

            // Project the filtered glasses to GlassDTO objects.
            foreach (Glasses glasses in glassesQuery)
            {
                var glassesClient = new GlassesDTO()
                {
                    ID = glasses.ID,
                    BrandName = glasses.BrandName,
                    Description = glasses.Description,
                    Price = glasses.Price,
                    Colour = glasses.Colour,
                    Style = glasses.Style,
                    Image = glasses.Image
                };

                glassesClientList.Add(glassesClient);
            }

            return View(glassesClientList);
        }


        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Details(int id)
        {
            var glasses = _context.Glasses.Find(id);
            var glassesDTO = new GlassesDTO
            {
                ID = glasses.ID,
                BrandName = glasses.BrandName,
                Description = glasses.Description,
                Price = glasses.Price,
                Colour = glasses.Colour,
                Style = glasses.Style,
                Image = glasses.Image
            };
            return View(glassesDTO);
        }


        public async Task<PartialViewResult> GenerateImage(string glasses)
        {
            var user = await _userManager.GetUserAsync(User);
            string imgB64 = null;
            if (user != null)
            {
                var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserID == user.Id);
                if (profile.ImageID != null)
                {
                    var uploadedImage = await _context.UploadedImages.FirstOrDefaultAsync(p => p.ID == profile.ImageID);
                    imgB64 = Convert.ToBase64String(uploadedImage.Image);
                }
            }

            if (user == null)
            {
                ViewData["error"] = "Please login to use this feature";
                return PartialView("_RenderPartial");
            }

            if (imgB64 == null)
            {
                ViewData["error"] = "Please upload a portrait";
                return PartialView("_RenderPartial");
            }

            if (!PythonEngine.IsInitialized)
            {
                Runtime.PythonDLL = Environment.GetEnvironmentVariable("Python_Runtime");
                PythonEngine.Initialize();
            }
            using (PyModule scope = Py.CreateScope())
            {
                // Injecting variables directly into the code string
                string code = $@"
import cv2
import numpy as np
import base64

# Load the face detection model
face = cv2.CascadeClassifier('./Resources/Detection/haarcascade_frontalface_default.xml')

# Decode the base64 image
img_decode = base64.b64decode('{imgB64}')
image = np.frombuffer(img_decode, np.uint8)
img = cv2.imdecode(image, cv2.IMREAD_COLOR)
gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
img_gray = face.detectMultiScale(gray, 1.09, 7)
frame = img.copy()
# Correctly form the path and load the glasses image
glasses_path = './wwwroot/{glasses}'
glasses = cv2.imread(glasses_path, cv2.IMREAD_UNCHANGED)

def put_glasses_on_face(glasses, fc, x, y, w, h):
    face_width = w
    face_height = h

    glasses_width = face_width + 1
    glasses_height = int(0.50 * face_height) + 1
    glasses_resized = cv2.resize(glasses, (glasses_width, glasses_height))
    
    for i in range(glasses_height):
        for j in range(glasses_width):
            if glasses_resized[i, j][3] != 0:  
                for k in range(3): 
                    if glasses[i][j][k] < 235:
                        fc[y + i - int(-0.20 * face_height)][x + j][k] = glasses_resized[i, j][k]
    return fc

for (x, y, w, h) in img_gray:
    frame = put_glasses_on_face(glasses, frame, x, y, w, h)

cv2.imwrite('./wwwroot/images/render_with_glasses.png', frame)
cv2.destroyAllWindows()
";
                scope.Exec(code);
            }
            PythonEngine.Shutdown();
            ViewData["renderedImage"] = "\\images\\render_with_glasses.png";
            ViewData["brandName"] = "Render";
            return PartialView("_RenderPartial");
        }


        [HttpGet]
        public async Task<IActionResult> DownloadImage()
        {
            var filePath = "./wwwroot/images/render.jpg"; // Path to the generated image
            if (System.IO.File.Exists(filePath))
            {
                var memoryStream = new MemoryStream();
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    await stream.CopyToAsync(memoryStream);
                }
                memoryStream.Position = 0; // Reset the memory stream position to allow for reading

                var fileName = "ARGeneratedImage.jpg";

                // Return the file with the appropriate MIME type
                return File(memoryStream, "image/jpeg", fileName);
            }
            else
            {
                // If the file doesn't exist, you might want to redirect to an error page or return a NotFound result
                return NotFound("The requested image does not exist.");
            }
        }




        [AjaxOnly]
        public PartialViewResult RenderDefault(string glasses, string brandName)
        {
            ViewData["renderedImage"] = $"\\{glasses}";
            ViewData["brandName"] = brandName;
            return PartialView("_RenderPartial");
        }

        
        public ActionResult AddToCart(int id, int qty)
        {
            var glass = _context.Glasses.Find(id);
            var glassesDTO = new GlassesDTO
            {
                ID = glass.ID,
                BrandName = glass.BrandName,
                Description = glass.Description,
                Price = glass.Price,
                Colour = glass.Colour,
                Style = glass.Style,
                Image = glass.Image,
            };
            if (id == null)
            {
                return View();
            }
            qty = 1;
            CartItem cartItem = new CartItem
            {
                ID = glassesDTO.ID,
                BrandName = glassesDTO.BrandName,
                Description = glassesDTO.Description,
                Image = glassesDTO.Image,
                Price = glassesDTO.Price,
                Quantity = qty,
                IsPurchased = false // Initially, the game is not purchased
            };

            List<CartItem> cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("cart") ?? new List<CartItem>();
            var existingItem = cart.Find(x => x.ID == id);

            if (existingItem != null)
            {
                existingItem.Quantity += qty;
            }
            else
            {
                cart.Add(cartItem);
            }

            HttpContext.Session.SetObjectAsJson("cart", cart);
            return RedirectToAction("Index","Home");
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int glassId)
        {
            var cartItems = HttpContext.Session.GetObjectFromJson<List<CartItem>>("cart") ?? new List<CartItem>();

            var itemToUpdate = cartItems.FirstOrDefault(item => item.ID == glassId);
            if (itemToUpdate != null)
            {
                itemToUpdate.Quantity -= 1;
                if (itemToUpdate.Quantity <= 0)
                {
                    cartItems.Remove(itemToUpdate);
                }

                HttpContext.Session.SetObjectAsJson("cart", cartItems);
            }

            return RedirectToAction("Checkout");
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            var cartItems = HttpContext.Session.GetObjectFromJson<List<CartItem>>("cart") ?? new List<CartItem>();
            var grandTotal = cartItems.Sum(item => item.TotalPrice);

            var viewModel = new CheckoutViewModel
            {
                CartItems = cartItems,
                PaymentInfo = new PaymentInfo(), // Initialize empty payment info
                GrandTotal = grandTotal
            };

            return View(viewModel);
        }
    }
}