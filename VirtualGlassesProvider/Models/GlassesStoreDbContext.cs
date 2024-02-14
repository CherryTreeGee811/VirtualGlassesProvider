using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace VirtualGlassesProvider.Models
{
    public class GlassesStoreDbContext : IdentityDbContext
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Glasses>().HasData(
               new Glasses { ID = 1, BrandName = "Rayban", Description = "Black Colour Squared shaped Rayban Sunglasses", Price = 10.99m, Colour="Black", Style="Square", Image= "GlassesImage/glasses.png" },
               new Glasses { ID = 2, BrandName = "Rayban", Description = "Blue Colour Circular shaped Rayban Sunglasses", Price = 10.99m, Colour = "Blue", Style = "Circular", Image = "GlassesImage/glassesbluerayban.png" }
            );
        }


        #region deleteTestClient
        /// <summary>
        /// Upon program execution TestClient data should be cleared from the database to allow
        /// repeated simulation of account registration in unit tests
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static async Task DeleteTestClient(IServiceProvider serviceProvider)
        {
            UserManager<IdentityUser> userManager =
                serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            var signUpTestClient = await userManager.FindByEmailAsync("TestClient@Sharklasers.com");

            if (signUpTestClient != null)
            {
                await userManager.DeleteAsync(signUpTestClient);
            }
        }
        #endregion


        public DbSet<Glasses> Glasses { get; set; }
        
        
        public GlassesStoreDbContext(DbContextOptions<GlassesStoreDbContext> options) : base(options)
        { }
    }
}
