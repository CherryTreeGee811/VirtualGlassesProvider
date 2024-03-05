using System.ComponentModel.DataAnnotations;
using VirtualGlassesProvider.Services;

namespace VirtualGlassesProvider.Models
{
    public class PaymentInfo
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
}
