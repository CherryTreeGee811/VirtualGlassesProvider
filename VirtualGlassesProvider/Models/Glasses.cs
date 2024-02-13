using System.ComponentModel.DataAnnotations;

namespace VirtualGlassesProvider.Models
{
    public class Glasses
    {
        [Key]
        public int glassesID { get; set; }

        public string glassesBrandName { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string color { get; set; }
        public string Style { get; set; }
        public string Image {  get; set; }
    }
}
