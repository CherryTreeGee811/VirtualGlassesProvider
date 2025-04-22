namespace VirtualGlassesProvider.Models.ViewModels
{
    public class CheckoutViewModel
    {
        public ICollection<CartItem>? CartItems { get; set; }


        public PaymentInfo? PaymentInfo { get; set; }


        public decimal GrandTotal { get; set; }
    }
}
