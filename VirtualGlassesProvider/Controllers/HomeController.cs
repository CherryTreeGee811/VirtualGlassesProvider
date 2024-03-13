using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using VirtualGlassesProvider.Models;
using VirtualGlassesProvider.Models.DataAccess;
using VirtualGlassesProvider.Models.DTOs;
using Python.Runtime;
using VirtualGlassesProvider.CustomAttributes;
using VirtualGlassesProvider.Services;
using VirtualGlassesProvider.Models.ViewModels;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;


namespace VirtualGlassesProvider.Controllers
{
    public sealed class HomeController : Controller
    {
        private readonly GlassesStoreDbContext _context;
        private const int PageSize = 10;
        private readonly UserManager<User> _userManager;
        private readonly AesEncryptionService _aesEncryptionService;


        public HomeController(GlassesStoreDbContext context, UserManager<User> userManager, AesEncryptionService aesEncryptionService)
        {
            _context = context;
            _userManager = userManager;
            _aesEncryptionService = aesEncryptionService;
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


        [AjaxOnly]
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
                var runtime = "";
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    runtime = Environment.GetEnvironmentVariable("Python_Runtime");
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    runtime = "/usr/lib/x86_64-linux-gnu/libpython3.10.so.1.0";
                }
                Runtime.PythonDLL = runtime;
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
                    fc[y + i - int(-0.20 * face_height)][x + j][k] = glasses_resized[i, j][k]
    return fc

for (x, y, w, h) in img_gray:
    frame = put_glasses_on_face(glasses, frame, x, y, w, h)

cv2.imwrite('./wwwroot/images/render.jpg', frame)
cv2.destroyAllWindows()
";
                scope.Exec(code);
            }
            PythonEngine.Shutdown();
            ViewData["renderedImage"] = "\\images\\render.jpg";
            ViewData["brandName"] = "Render";
            return PartialView("_RenderPartial");
        }


        [AjaxOnly]
        public IActionResult ApplyGlassesFilter(string glasses)
        {
            if (!PythonEngine.IsInitialized)
            {
                var runtime = "";
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    runtime = Environment.GetEnvironmentVariable("Python_Runtime");
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    runtime = "/usr/lib/x86_64-linux-gnu/libpython3.10.so.1.0";
                }
                Runtime.PythonDLL = runtime;
                PythonEngine.Initialize();
            }
            using (PyModule scope = Py.CreateScope())
            {
                string code = @"
import cv2
import numpy as np

def put_glass(glass, fc, x, y, w, h):
    face_width = w
    face_height = h

    hat_width = face_width + 1
    hat_height = int(0.50 * face_height) + 1

    glass = cv2.resize(glass, (hat_width, hat_height))

    for i in range(hat_height):
        for j in range(hat_width):
            for k in range(3):
                if glass[i][j][k] < 235:
                    fc[y + i - int(-0.20 * face_height)][x + j][k] = glass[i][j][k]
    return fc

face = cv2.CascadeClassifier('haarcascade_frontalface_default.xml')
glasses_path = './wwwroot/{glasses}'
glass = cv2.imread(glasses_path)

webcam = cv2.VideoCapture(0)
while True:
    size = 4
    (rval, im) = webcam.read()
    if not rval:
        break
    im = cv2.flip(im, 1, 0)
    gray = cv2.cvtColor(im, cv2.COLOR_BGR2GRAY)
    fl = face.detectMultiScale(gray, 1.19, 7)

    for (x, y, w, h) in fl:
        im = put_glass(glass, im, x, y, w, h)

    cv2.imshow('Hat & glasses', im)
    key = cv2.waitKey(30) & 0xff
    if key == 27:  # The Esc key
        break

webcam.release()
cv2.destroyAllWindows()
";
                scope.Exec(code);
            }
            PythonEngine.Shutdown();

            // Return some response to indicate the operation has completed
            return Ok("Filter applied successfully.");
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


        [HttpGet]
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


        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Checkout()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var paymentinfo = await _context.PaymentInfo.FirstOrDefaultAsync(p => p.UserID == user.Id);

            var cartItems = HttpContext.Session.GetObjectFromJson<List<CartItem>>("cart") ?? new List<CartItem>();
            var grandTotal = cartItems.Sum(item => item.TotalPrice);
            CheckoutViewModel viewModel = null;
            if(paymentinfo == null)
            {
                viewModel = new CheckoutViewModel
                {
                    CartItems = cartItems,
                    PaymentInfo = new PaymentInfo(), // Initialize empty payment info
                    GrandTotal = grandTotal
                };
            }
            else
            {
                if(!String.IsNullOrEmpty(paymentinfo.CardHolderName))
                {
                    paymentinfo.CardNumber = _aesEncryptionService.Decrypt(paymentinfo.CardNumber);
                }

                if (!String.IsNullOrEmpty(paymentinfo.CVV))
                {
                    paymentinfo.CardNumber = _aesEncryptionService.Decrypt(paymentinfo.CVV);
                }

                viewModel = new CheckoutViewModel
                {
                    CartItems = cartItems,
                    PaymentInfo = paymentinfo, // Initialize existing payment info
                    GrandTotal = grandTotal
                };
            }
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutViewModel viewModel)
        {

            if (ModelState.IsValid)
            {
                var cartItems = HttpContext.Session.GetObjectFromJson<List<CartItem>>("cart");
                if (cartItems != null && cartItems.Any())
                {
                    // Create and save the invoice
                    var invoice = new Invoice
                    {
                        UserId = User?.Identity.Name ?? string.Empty, // Or however you get the user ID
                        InvoiceDate = DateTime.Now,
                        Bill = cartItems.Sum(item => item.TotalPrice),
                        PaymentMethod = "Card"
                    };
                    _context.Invoices.Add(invoice);
                    await _context.SaveChangesAsync();

                    // Create and save each order
                    foreach (var item in cartItems)
                    {

                        var order = new Order
                        {
                            GlassId = item.ID,
                            BrandName = item.BrandName,
                            Quantity = item.Quantity,
                            UnitPrice = item.Price,
                            TotalPrice = item.TotalPrice,
                            OrderDate = DateTime.Now,
                            Status = "Now Processing",
                            IsPurchased = true,
                            InvoiceId = invoice.Id // Use the Invoice Id
                        };

                        _context.Orders.Add(order);
                    }
                    await _context.SaveChangesAsync();

                    // Clear the session cart
                    HttpContext.Session.Remove("cart");

                    var confirmationViewModel = new OrderConfirmationViewModel
                    {
                        InvoiceDetails = invoice,
                        OrderItems = await _context.Orders
                                       .Where(o => o.InvoiceId == invoice.Id)
                                       .ToListAsync()
                    };

                    // Use TempData or a similar mechanism to pass data to the redirection target
                    TempData["OrderConfirmation"] = JsonConvert.SerializeObject(confirmationViewModel);


                    // Redirect to an order confirmation page
                    return RedirectToAction("OrderConfirmation");
                }
            }
            else
            {
                viewModel.CartItems = HttpContext.Session.GetObjectFromJson<List<CartItem>>("cart") ?? new List<CartItem>();
                viewModel.GrandTotal = viewModel.CartItems.Sum(item => item.TotalPrice);
            }

            // If model state is not valid, or cart is empty, return to the same view
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult OrderConfirmation()
        {
            if (TempData["OrderConfirmation"] is string serializedConfirmationViewModel)
            {
                var viewModel = JsonConvert.DeserializeObject<OrderConfirmationViewModel>(serializedConfirmationViewModel);
                return View(viewModel);
            }

            // Redirect to a different page or show an error if the order details are not available
            return RedirectToAction("Index"); // or return View("Error");
        }

        public async Task<IActionResult> OrderDetail()
        {
            var userId = User?.Identity.Name ?? string.Empty;

            var userOrders = await _context.Orders
                                           .Include(o => o.Invoice) // Include the Invoice in the query
                                           .Where(o => o.Invoice.UserId == userId)
                                           .ToListAsync();

            return View(userOrders);
        }
    }
}