using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace VirtualGlassesProvider.Models
{
    public class Profiles
    {
        [Key]
        public int ID { get; set; }


        public string? DisplayName { get; set; }


        [ForeignKey("IdentityUser")]
        public string? UserID { get; set; }


        [ForeignKey("UploadedImages")]
        public int? ImageID { get; set; }
    }
}
