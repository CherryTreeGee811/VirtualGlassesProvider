using System.ComponentModel.DataAnnotations;


namespace VirtualGlassesProvider.Models
{
    public class UploadedImages
    {
        [Key]
        public int ID { get; set; }


        public byte[]? Image { get; set; }
    }
}
