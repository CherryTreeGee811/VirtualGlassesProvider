using System.ComponentModel.DataAnnotations.Schema;

namespace VirtualGlassesProvider.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int GlassId { get; set; }
        public string BrandName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public bool IsPurchased { get; set; }
        public string Status { get; set; } // "Processed", "Unprocessed"
        [ForeignKey("Invoice")]
        public int InvoiceId { get; set; } // Foreign key to Invoice
        public Invoice Invoice { get; set; }
    }
}
