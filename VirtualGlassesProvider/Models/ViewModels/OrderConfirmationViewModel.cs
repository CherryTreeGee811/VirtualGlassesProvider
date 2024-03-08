namespace VirtualGlassesProvider.Models.ViewModels
{
    public class OrderConfirmationViewModel
    {
        public Invoice InvoiceDetails { get; set; }
        public List<Order> OrderItems { get; set; }
    }
}
