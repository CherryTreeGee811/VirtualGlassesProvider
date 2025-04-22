using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using VirtualGlassesProvider.Models;
using VirtualGlassesProvider.Models.DataAccess;
using VirtualGlassesProvider.Models.DTOs;
using VirtualGlassesProvider.Services;
using VirtualGlassesProvider.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;


namespace VirtualGlassesProvider.Controllers
{
    public sealed class HomeController(
            GlassesStoreDbContext context,
            UserManager<User> userManager,
            AesEncryptionService aesEncryptionService) : Controller
    {
        private readonly GlassesStoreDbContext _context = context;
        private readonly UserManager<User> _userManager = userManager;
        private readonly AesEncryptionService _aesEncryptionService = aesEncryptionService;
        private const int PageSize = 10;


        [HttpGet]
        public async Task<IActionResult> Index(int pageNumber = 1)
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
            if (string.IsNullOrEmpty(searchString))
            {
                return RedirectToAction("Index");
            }

            var glassesClientList = new List<GlassesDTO>();

            // Start with all glasses if the search string is not null or empty.
            var glassesQuery = _context.Glasses.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                // Modify the query to filter by any of the conditions.
                glassesQuery = glassesQuery.Where(g =>
                    (g.BrandName != null && g.BrandName.Contains(searchString)) ||
                    (g.Style != null && g.Style.Contains(searchString)) ||
                    (g.Colour != null && g.Colour.Contains(searchString)) ||
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
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var family = await _context.FamilyMembers
                   .Where(u => u.UserID == user.Id)
                   .Select(f => new FamilyARViewModel
                   {
                       ID = f.ID,
                       Name = $"{f.FirstName} {f.LastName}"
                   }).ToListAsync();
                if (family != null)
                {
                    ViewBag.Members = family;
                }
            }
            var glasses = await _context.Glasses.FindAsync(id);

            if (glasses == null)
            {
                return NotFound();
            }

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


        [HttpGet]
        public async Task<string> GetPortrait(string? entity)
        {
            var user = await _userManager.GetUserAsync(User);
            string? imgB64 = null;
            if (user != null)
            {
                if (string.Equals(entity, "self"))
                {
                    var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserID == user.Id);
                    if (profile != null || profile?.ImageID != null)
                    {
                        var uploadedImage = await _context.UploadedImages.FirstOrDefaultAsync(p => p.ID == profile.ImageID);
                        if (uploadedImage?.Image != null)
                        {
                            imgB64 = Convert.ToBase64String(uploadedImage.Image);
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(entity))
                {
                    bool parseSuccessfull = int.TryParse(entity, out int familyID);
                    if (parseSuccessfull)
                    {
                        var familyMember = await _context.FamilyMembers.Where(u => u.UserID == user.Id).Where(f => f.ID == familyID).FirstAsync();
                        if (familyMember.ImageID != null)
                        {
                            var uploadedImage = await _context.UploadedImages.FirstOrDefaultAsync(p => p.ID == familyMember.ImageID);
                            if (uploadedImage?.Image != null)
                            {
                                imgB64 = Convert.ToBase64String(uploadedImage.Image);
                            }
                        }
                    }
                }
            }

            if (user == null)
            {
                return "Please login to use this feature";
            }

            if (imgB64 == null)
            {
                return "Please upload a portrait";
            }

            return "data:image/jpg;base64," + imgB64;
        }


        [Authorize]
        [HttpGet]
        public IActionResult AddToCart(int id, int qty, string source)
        {
            var glass = _context.Glasses.Find(id);

            if (glass == null)
            {
                return NotFound("The specified glasses item was not found.");
            }

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

            qty = 1;
            var cartItem = new CartItem
            {
                ID = glassesDTO.ID,
                BrandName = glassesDTO.BrandName,
                Description = glassesDTO.Description,
                Image = glassesDTO.Image,
                Price = glassesDTO.Price,
                Quantity = qty,
                IsPurchased = false // Initially, the game is not purchased
            };

            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("cart") ?? new List<CartItem>();
            var existingItem = cart.Find(x => x.ID == id);

            if (existingItem != null)
            {
                existingItem.Quantity += qty;
            }
            else
            {
                cart.Add(cartItem);
            }

            TempData["AddedToCartMessage"] = "Glasses successfully added to cart!";

            HttpContext.Session.SetObjectAsJson("cart", cart);
            if (source == "details")
            {
                return RedirectToAction("Details", "Home", new { id });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }


        [Authorize]
        [HttpPost]
        public IActionResult RemoveFromCart(int glassId)
        {
            var cartItems = HttpContext.Session.GetObjectFromJson<HashSet<CartItem>>("cart") ?? new HashSet<CartItem>();

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
        public async Task<IActionResult> Checkout()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var paymentinfo = await _context.PaymentInfo.FirstOrDefaultAsync(p => p.UserID == user.Id);

            var cartItems = HttpContext.Session.GetObjectFromJson<HashSet<CartItem>>("cart") ?? new HashSet<CartItem>();
            var grandTotal = cartItems.Sum(item => item.TotalPrice);
            CheckoutViewModel? viewModel = null;
            if (paymentinfo == null)
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
                if (!string.IsNullOrEmpty(paymentinfo.CardNumber))
                {
                    paymentinfo.CardNumber = _aesEncryptionService.Decrypt(paymentinfo.CardNumber);
                }

                if (!string.IsNullOrEmpty(paymentinfo.CVV))
                {
                    paymentinfo.CVV = _aesEncryptionService.Decrypt(paymentinfo.CVV);
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


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var cartItems = HttpContext.Session.GetObjectFromJson<List<CartItem>>("cart");
                if (cartItems != null && cartItems.Count > 0)
                {
                    var userId = User?.Identity?.Name;

                    if (string.IsNullOrEmpty(userId))
                    {
                        return NotFound("User ID is null or empty.");
                    }

                    // Create and save the invoice
                    var invoice = new Invoice
                    {
                        UserId = userId,
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
                    TempData["OrderConfirmation"] = JsonSerializer.Serialize(confirmationViewModel);

                    // Redirect to an order confirmation page
                    return RedirectToAction("OrderConfirmation");
                }
            }
            else
            {
                viewModel.CartItems = HttpContext.Session.GetObjectFromJson<HashSet<CartItem>>("cart");
                if (viewModel.CartItems != null)
                {
                    viewModel.GrandTotal = viewModel.CartItems.Sum(item => item.TotalPrice);
                }
                else
                {
                    viewModel.GrandTotal = 0;
                }
            }

            // If model state is not valid, or cart is empty, return to the same view
            return View(viewModel);
        }


        [Authorize]
        [HttpGet]
        public IActionResult OrderConfirmation()
        {
            if (TempData["OrderConfirmation"] is string serializedConfirmationViewModel)
            {
                var viewModel = JsonSerializer.Deserialize<OrderConfirmationViewModel>(serializedConfirmationViewModel);
                return View(viewModel);
            }

            // Redirect to a different page or show an error if the order details are not available
            return RedirectToAction("Index"); // or return View("Error");
        }


        [Authorize]
        public async Task<IActionResult> OrderDetail()
        {
            var userId = User?.Identity?.Name;

            if (string.IsNullOrEmpty(userId))
            {
                return NotFound("User ID is null or empty.");
            }

            var userOrders = await _context.Orders
                .Include(o => o.Invoice) // Include the Invoice in the query
                .Where(o => o.Invoice != null && o.Invoice.UserId == userId) // Ensure Invoice is not null
                .ToListAsync();

            return View(userOrders);
        }


        [Authorize]
        public async Task<IActionResult> WishList()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null || string.IsNullOrEmpty(user.Id))
            {
                return NotFound("User ID is null or empty.");
            }

            var wishList = await _context.WishLists
                .Include(w => w.WishListItems)
                .Where(w => w.User != null && w.User.Id == user.Id)
                .FirstOrDefaultAsync();

            if (wishList == null)
            {
                wishList = new WishLists { User = user, WishListItems = new HashSet<WishListItems>() };
                _context.WishLists.Add(wishList);
                await _context.SaveChangesAsync();
            }

            if (wishList.WishListItems == null)
            {
                wishList.WishListItems = new HashSet<WishListItems>();
            }

            if (wishList.WishListItems.Count > 0)
            {

                var glasses = new List<Glasses>();
                foreach (Glasses glass in _context.Glasses)
                {
                    var glasses1 = new Glasses()
                    {
                        ID = glass.ID,
                        BrandName = glass.BrandName,
                        Description = glass.Description,
                        Price = glass.Price,
                        Colour = glass.Colour,
                        Style = glass.Style,
                        Image = glass.Image,

                    };

                    glasses.Add(glasses1);
                }



                foreach (var glass in glasses)
                {
                    var item = wishList.WishListItems.SingleOrDefault(wi => wi.GlassesID == glass.ID); ;
                    if (item != null)
                    {
                        wishList.WishListItems.Remove(item);
                        item.Glasses = glass;
                        wishList.WishListItems.Add(item);
                    }
                }



            }
            else
            {
                wishList.WishListItems = new HashSet<WishListItems>();
            }

            ViewBag.UserName = user.UserName;

            return View(wishList);
        }


        [Authorize]
        public async Task<IActionResult> AddToWishList(int ID)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null || string.IsNullOrEmpty(user.Id))
            {
                return NotFound("User ID is null or empty.");
            }

            var isInWishlist = await _context.WishListItems
                .AnyAsync(wli => (wli.WishLists != null) &&
                    (wli.WishLists.User != null) &&
                    (wli.WishLists.User.Id == user.Id) &&
                    (wli.GlassesID == ID));


            if (!isInWishlist)
            {
                var wishList = await _context.WishLists
                    .Include(w => w.WishListItems)
                    .FirstOrDefaultAsync(w => (w.User != null) && (w.User.Id == user.Id));

                if (wishList == null)
                {
                    wishList = new WishLists { User = user };
                    _context.WishLists.Add(wishList);
                    await _context.SaveChangesAsync();
                }

                var item = new WishListItems { GlassesID = ID, WishLists = wishList };

                _context.WishListItems.Add(item);
                await _context.SaveChangesAsync();

                TempData["AddedToWishlistMessage"] = "Glasses successfully added to wishlist!";
            }
            else
            {
                TempData["AddedToWishlistMessage"] = "Glasses are already in the wishlist!";
            }

            return RedirectToAction("Index", "Home");
        }


        [Authorize]
        public async Task<IActionResult> RemoveFromWishList(int ID, string page)
        {
            var user = await _userManager.GetUserAsync(User);

            var wishList = await _context.WishLists.Include(w => w.WishListItems).Where(w => w.User == user).FirstOrDefaultAsync();

            var item = wishList?.WishListItems?.SingleOrDefault(wi => wi.GlassesID == ID);

            if (item != null)
            {
                _context.WishListItems.Remove(item);
                await _context.SaveChangesAsync();
            }

            if (page.Equals("Index"))
            {
                return RedirectToAction("Index", "Home", new { id = ID });
            }
            else
            {
                return RedirectToAction("WishList");
            }
        }


        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new { status = "healthy" });
        }
    }
}