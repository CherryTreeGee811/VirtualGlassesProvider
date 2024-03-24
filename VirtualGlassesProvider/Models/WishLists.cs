using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VirtualGlassesProvider.Models
{
    public class WishLists
    {
        [Key]
        public int ID { get; set; }


        [ForeignKey("User")]
        public string? UserID { get; set; }


        public virtual User? User { get; set; }


        public ICollection<WishListItems> WishListItems { get; set; }
    }
}
