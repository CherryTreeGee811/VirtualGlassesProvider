using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace VirtualGlassesProvider.Models
{
    public class FamilyMember
    {
        [Key]
        public int ID { get; set; }


        [ForeignKey("User")]
        public string? UserID { get; set; }


        public virtual User? User { get; set; }


        [Display(Name = "First Name")]
        public string? FirstName { get; set; }



        [Display(Name = "Last Name")]
        public string? LastName { get; set; }


        [Display(Name = "Address")]
        public string? Address { get; set; }


        [Display(Name = "Email")]
        public string? Email { get; set; }


        [Display(Name = "Relationship")]
        public string? Relationship { get; set; }


        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }


        [ForeignKey("UploadedImages")]
        public int? ImageID { get; set; }
    }
}
