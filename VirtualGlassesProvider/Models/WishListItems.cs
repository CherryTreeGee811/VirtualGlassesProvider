using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace VirtualGlassesProvider.Models
{
    public class WishListItems
    {
        [Key]
        public int ID { get; set; }


        public int GlassesID { get; set; }


        [NotMapped]
        public Glasses? Glasses { get; set; }


        public WishLists? WishLists { get; set; }
    }
}
