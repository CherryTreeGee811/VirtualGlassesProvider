using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace VirtualGlassesProvider.Models
{
    public class GlassesStoreDbContext : IdentityDbContext
    {
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Glasses>().HasData(
               new Glasses { glassesID = 1, glassesBrandName = "Rayban", Description = "Black Colour Sqaured shaped Rayban Sunglasses", Price = 10.99m, color="Black", Style="Square", Image= "GlassesImage/glasses.png" },
               new Glasses { glassesID = 2, glassesBrandName = "Rayban", Description = "Blue Colour Circular shaped Rayban Sunglasses", Price = 10.99m, color = "Blue", Style = "Circular", Image = "GlassesImage/glassesbluerayban.png" }
               );

        }
        public DbSet<Glasses> Glasses { get; set; }
        public GlassesStoreDbContext(DbContextOptions<GlassesStoreDbContext> options) : base(options)
        { }
    }
}
