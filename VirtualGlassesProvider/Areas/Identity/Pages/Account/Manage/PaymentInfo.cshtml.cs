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
    public class PaymentInfoModel : PageModel
    {
        private readonly UserManager<VirtualGlassesProvider.Models.User> _userManager;
        private readonly SignInManager<VirtualGlassesProvider.Models.User> _signInManager;
        private readonly GlassesStoreDbContext _context;
        private readonly AesEncryptionService _aesEncryptionService;
        public PaymentInfoModel(
            UserManager<VirtualGlassesProvider.Models.User> userManager,
            SignInManager<VirtualGlassesProvider.Models.User> signInManager,
            GlassesStoreDbContext context, AesEncryptionService aesEncryptionService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _aesEncryptionService = aesEncryptionService;
        }
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }


        [BindProperty(SupportsGet = true)]
        public InputModel Input { get; set; }


        public sealed class InputModel
        {

            [Required(ErrorMessage = "Card Holder Name is required")]
            [Display(Name = "Card Holder Name")]
            public string CardHolderName { get; set; }
            [Required(ErrorMessage = "Card Number is required")]
            [CreditCard(ErrorMessage = "Invalid Card Number")]
            [Display(Name = "Card Number")]
            public string CardNumber { get; set; }
            [Required(ErrorMessage = "CVV is required")]
            [RegularExpression(@"^\d{3,4}$", ErrorMessage = "Invalid CVV")]
            [Display(Name = "CVV")]
            public string CVV { get; set; }
            [Required(ErrorMessage = "Expiry Date is required")]
            [RegularExpression(@"^(0[1-9]|1[0-2])\/?([0-9]{2})$", ErrorMessage = "Invalid Expiry Date. Format MM/YY")]
            [FutureDate(ErrorMessage = "Expiry Date cannot be in the past")]
            [Display(Name = "Expiry Date")]
            public string ExpiryDate { get; set; }
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var paymentinfo = await _context.PaymentInfo.FirstOrDefaultAsync(p => p.UserID == user.Id);

            if (paymentinfo == null)
            {
                paymentinfo = new PaymentInfo();
                paymentinfo.UserID = user.Id;
                _context.Add(paymentinfo);
                await _context.SaveChangesAsync();
            }
            if (paymentinfo.CardHolderName != null)
            {
                Input.CardHolderName = paymentinfo.CardHolderName; 
            }
            if (paymentinfo.CardNumber != null)
            {
                Input.CardNumber = _aesEncryptionService.Decrypt(paymentinfo.CardNumber);
            }
            if (paymentinfo.CVV != null)
            {
                Input.CVV = _aesEncryptionService.Decrypt(paymentinfo.CVV); 
            }
            if (paymentinfo.ExpiryDate != null)
            {
                Input.ExpiryDate = paymentinfo.ExpiryDate;
            }
            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var paymentinfo = await _context.PaymentInfo.FirstOrDefaultAsync(p => p.UserID == user.Id);

            paymentinfo.UserID = user.Id;
            paymentinfo.CVV = _aesEncryptionService.Encrypt(Input.CVV);
            paymentinfo.CardHolderName = Input.CardHolderName;
            paymentinfo.ExpiryDate = Input.ExpiryDate;
            paymentinfo.CardNumber = _aesEncryptionService.Encrypt(Input.CardNumber);
            _context.PaymentInfo.Update(paymentinfo);
            _context.SaveChanges();

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your Payment Info has been updated";
            return RedirectToPage();
        }
    }
}
