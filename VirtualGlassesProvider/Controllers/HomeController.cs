using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using VirtualGlassesProvider.Models;
using VirtualGlassesProvider.Models.DataAccess;
using VirtualGlassesProvider.Models.DTOs;
using VirtualGlassesProvider.CustomAttributes;
using VirtualGlassesProvider.Services;
using VirtualGlassesProvider.Models.ViewModels;
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
                if(family != null)
                {
                    ViewBag.Members = family;
                }
            }
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
        public async Task<string> GetPortrait(string? entity)
        {
            var user = await _userManager.GetUserAsync(User);
            string imgB64 = null;
            Console.WriteLine(entity);
            if (user != null)
            {
                if (entity.Equals("self"))
                {
                    var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserID == user.Id);
                    if (profile != null || profile.ImageID != null)
                    {
                        var uploadedImage = await _context.UploadedImages.FirstOrDefaultAsync(p => p.ID == profile.ImageID);
                        imgB64 = Convert.ToBase64String(uploadedImage.Image);
                    }
                }
                else if (!String.IsNullOrEmpty(entity))
                {
                    bool parseSuccessfull = int.TryParse(entity, out int familyID);
                    if (parseSuccessfull)
                    {
                        var familyMember = await _context.FamilyMembers.Where(u => u.UserID == user.Id).Where(f => f.ID == familyID).FirstAsync();
                        if (familyMember.ImageID != null)
                        {
                            var uploadedImage = await _context.UploadedImages.FirstOrDefaultAsync(p => p.ID == familyMember.ImageID);
                            imgB64 = Convert.ToBase64String(uploadedImage.Image);
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

        public async Task<IActionResult> WishList()
        {
            var user = await _userManager.GetUserAsync(User);

            WishLists wishList = await _context.WishLists.Include(w => w.WishListItems).Where(w => w.User.Id == user.Id).FirstOrDefaultAsync();

            if (wishList == null)
            {
                wishList = new WishLists { User = user };
                _context.WishLists.Add(wishList);
                _context.SaveChanges();
            }

            if (wishList.WishListItems == null)
            {
                wishList.WishListItems = new List<WishListItems>();
            }

            if (wishList.WishListItems.Count > 0)
            {

                List<Glasses> glasses = new List<Glasses>();
                foreach (Glasses glass in _context.Glasses)
                {
                    Glasses glasses1 = new Glasses()
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
                    WishListItems item = wishList.WishListItems.SingleOrDefault(wi => wi.GlassesID == glass.ID); ;
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
                wishList.WishListItems = new List<WishListItems>();
            }

            ViewBag.UserName = user.UserName;

            return View(wishList);
        }
    
        public async Task<IActionResult> AddToWishList(int ID)
        {
            var user = await _userManager.GetUserAsync(User);

            WishLists wishList = await _context.WishLists.Include(w => w.WishListItems).Where(w => w.User.Id == user.Id).FirstOrDefaultAsync();

            if (wishList == null)
            {
                wishList = new WishLists { User = user };
                _context.WishLists.Add(wishList);
                await _context.SaveChangesAsync();
            }

            WishListItems item = new WishListItems { GlassesID = ID, WishLists = wishList };

            _context.WishListItems.Add(item);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}