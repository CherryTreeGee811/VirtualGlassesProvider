namespace VirtualGlassesProvider.Models.ViewModels
{
    public class OrderConfirmationViewModel
    {
        public Invoice? InvoiceDetails { get; set; }


        public ICollection<Order>? OrderItems { get; set; }
    }
}
