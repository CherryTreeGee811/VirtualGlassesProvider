namespace VirtualGlassesProvider.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal Bill { get; set; }
        public string PaymentMethod { get; set; }
    }
}
