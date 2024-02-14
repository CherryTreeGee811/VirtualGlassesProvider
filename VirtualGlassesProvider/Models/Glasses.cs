using System.ComponentModel.DataAnnotations;


namespace VirtualGlassesProvider.Models
{
    public sealed class Glasses
    {
        [Key]
        public int ID { get; set; }

        public string BrandName { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string Colour { get; set; }

        public string Style { get; set; }

        public string Image {  get; set; }
    }
}
