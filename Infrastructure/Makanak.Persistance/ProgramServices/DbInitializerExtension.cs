using Makanak.Persistance.Seeds;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Makanak.Persistance.Extensions
{
    public static class DbInitializerExtension
    {
        public static async Task SeedDatabaseAsync(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    await RoleSeeder.SeedRolesAsync(roleManager);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("DbInitializer");
                    logger.LogError(ex, "An error occurred while seeding roles.");
                }
            }
        }
    }
}