using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace VirtualGlassesProvider.Models
{
    public class GlassesStoreDbContext : IdentityDbContext
    {
        public GlassesStoreDbContext(DbContextOptions<GlassesStoreDbContext> options) : base(options)
        {
        }
    }
}
