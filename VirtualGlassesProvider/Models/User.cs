using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace VirtualGlassesProvider.Models
{
    public class User : IdentityUser
    {
        [NotMapped]
        public IList<string> RoleNames { get; set; }


        public PaymentInfo? PaymentInfo { get; set; }


        public Profiles? Profiles { get; set; }
        public WishLists? WishLists { get; set; }


        public ICollection<FamilyMember>? FamilyMembers { get; set; }

    }
}
