using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtualGlassesProvider.Services;

namespace VirtualGlassesProvider.Models
{
    public class PaymentInfo
    {
        [Key]
        public int id {  get; set; }
        [ForeignKey("User")]
        public string? UserID { get; set; }

        public virtual User? User { get; set; }
        [Display(Name = "Card Holder Name")]
        public string? CardHolderName { get; set; }
        [Display(Name = "Card Number")]
        public string? CardNumber { get; set; }
        [Display(Name = "CVV")]
        public string? CVV { get; set; }
        [Display(Name = "Expiry Date")]
        public string? ExpiryDate { get; set; }
    }
}
