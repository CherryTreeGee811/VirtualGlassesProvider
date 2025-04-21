using Microsoft.EntityFrameworkCore;
using VirtualGlassesProvider.Models.DataAccess;


namespace VisionVogue.Extensions
{
    /// <summary>
    /// Provides extension methods for applying database migrations.
    /// </summary>
    public static class MigrationExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();

            using var dbContext =
                scope.ServiceProvider.GetRequiredService<GlassesStoreDbContext>();

            dbContext.Database.Migrate();
        }
    }
}