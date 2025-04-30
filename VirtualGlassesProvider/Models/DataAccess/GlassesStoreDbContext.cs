using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace VirtualGlassesProvider.Models.DataAccess
{
    public class GlassesStoreDbContext(DbContextOptions<GlassesStoreDbContext> options)
        : IdentityDbContext<IdentityUser>(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Glasses>().HasData(
               new() { ID = 1, BrandName = "Rayban", Description = "Black Colour Squared shaped Rayban Sunglasses", Price = 10.99m, Colour = "Black", Style = "Square", Image = "GlassesImage/glasses.png" },
               new() { ID = 2, BrandName = "Rayban", Description = "Blue Colour Circular shaped Rayban Sunglasses", Price = 10.99m, Colour = "Blue", Style = "Circular", Image = "GlassesImage/glassesbluerayban.png" },
               new() { ID = 3, BrandName = "Rayban", Description = "Blue Tint Colour Squared shaped Rayban Sunglasses", Price = 10.99m, Colour = "Blue Tint", Style = "Square", Image = "GlassesImage/glasses-bluetint.png" },
               new() { ID = 4, BrandName = "Rayban", Description = "Green Colour Squared shaped Rayban Sunglasses", Price = 10.99m, Colour = "Green", Style = "Square", Image = "GlassesImage/glasses-green.png" },
               new() { ID = 5, BrandName = "Rayban", Description = "Red Colour Squared shaped Rayban Sunglasses", Price = 10.99m, Colour = "Red", Style = "Square", Image = "GlassesImage/glasses-red.png" },
               new() { ID = 6, BrandName = "Rayban", Description = "Clementine Colour Squared shaped Rayban Sunglasses", Price = 10.99m, Colour = "Clementine", Style = "Square", Image = "GlassesImage/glasses-clementine.png" },
               new() { ID = 7, BrandName = "Rayban", Description = "Grape Colour Squared shaped Rayban Sunglasses", Price = 10.99m, Colour = "Grape", Style = "Square", Image = "GlassesImage/glasses-grape.png" },
               new() { ID = 8, BrandName = "Rayban", Description = "Violin Colour Squared shaped Rayban Sunglasses", Price = 10.99m, Colour = "Violin", Style = "Square", Image = "GlassesImage/glasses-violin.png" }
            );

            builder.Entity<Profiles>()
                .HasOne(u => u.User)
                .WithOne(p => p.Profiles)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PaymentInfo>()
                .HasOne(u => u.User)
                .WithOne(p => p.PaymentInfo)
                .OnDelete(DeleteBehavior.Cascade);
             
            builder.Entity<WishLists>()
                .HasOne(u => u.User)
                .WithOne(w => w.WishLists)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<WishListItems>()
                .HasOne(wi => wi.WishLists)
                .WithMany(w => w.WishListItems)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<FamilyMember>()
                .HasOne(u => u.User)
                .WithMany(p => p.FamilyMembers)
                .OnDelete(DeleteBehavior.Cascade);
        }


        public static async Task CreateAdminUser(IServiceProvider serviceProvider)
        {
            var userManager =
                serviceProvider.GetRequiredService<UserManager<User>>();

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string adminUsername = "admin";
            string adminPassword = "Test1$";
            string adminRoleName = "Admin";


            // if role doesn't exist, create it
            if (await roleManager.FindByNameAsync(adminRoleName) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(adminRoleName));
            }

            // if admin username doesn't exist, create it and add it to role
            if (await userManager.FindByNameAsync(adminUsername) == null)
            {
                var admin = new User { UserName = adminUsername, Email = "admin@vv.com", NormalizedEmail = "ADMIN@VV.COM", EmailConfirmed = true };
                var result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, adminRoleName);
                }
            }
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
            UserManager<User> userManager =
                serviceProvider.GetRequiredService<UserManager<User>>();

            var signUpTestClient = await userManager.FindByEmailAsync("TestClient@Sharklasers.com");

            if (signUpTestClient != null)
            {
                await userManager.DeleteAsync(signUpTestClient);
            }
        }
        #endregion


        public DbSet<Glasses> Glasses { get; set; }


        public DbSet<Profiles> Profiles { get; set; }


        public DbSet<PaymentInfo> PaymentInfo { get; set; }


        public DbSet<UploadedImages> UploadedImages { get; set; }


        public DbSet<Invoice> Invoices { get; set; }


        public DbSet<Order> Orders { get; set; }


        public DbSet<WishLists> WishLists { get; set; }


        public DbSet<WishListItems> WishListItems { get; set; }


        public DbSet<FamilyMember> FamilyMembers { get; set; }
    }
}
