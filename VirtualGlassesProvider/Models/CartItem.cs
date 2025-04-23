using System.ComponentModel.DataAnnotations;


namespace VirtualGlassesProvider.Models
{
    public class CartItem
    {
        [Key]
        public int ID { get; set; }


        public string? BrandName { get; set; }


        public string? Description { get; set; }


        public string? Image { get; set; }


        public decimal Price { get; set; }


        public int Quantity { get; set; }


        public bool IsPurchased { get; set; }


        public decimal TotalPrice { get { return Price * Quantity; } }
    }
}
