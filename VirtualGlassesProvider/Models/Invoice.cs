using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace VirtualGlassesProvider.Models
{
    public class Invoice
    {
        [Key]
        public int Id { get; set; }


        [ForeignKey("User")]
        public string? UserId { get; set; }


        public DateTime InvoiceDate { get; set; }


        public decimal Bill { get; set; }


        public string? PaymentMethod { get; set; }
    }
}
