namespace VirtualGlassesProvider.Models
{
    public class CartItem
    {
        public int ID { get; set; }
        public string BrandName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public bool IsPurchased { get; set; }
        public decimal TotalPrice { get { return Price * Quantity; } }
    }
}
