using Makanak.Domain.Contracts.InitializerDB;
using Makanak.Domain.Models.Identity;
using Makanak.Persistance.Contexts;
using Makanak.Persistance.Seeds;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Makanak.Persistance.Implements.InitializerImplement
{
    public class DbInitialized(MakanakDbContext makanakDbContext , RoleManager<IdentityRole> roleManager , UserManager<ApplicationUser> userManager) : IDbInitializer
    {
        public async Task DataSeedAsync()
        {
            try
            {
                var pendingMigrations = await makanakDbContext.Database.GetPendingMigrationsAsync();
                if (pendingMigrations != null && pendingMigrations.Any())
                    await makanakDbContext.Database.MigrateAsync();
            }
            catch (Exception)
            {
                // Log the exception or handle it as needed
                throw;
            }
            await SeederAsync.SeedRolesAsync(roleManager);

            await SeederAsync.SeedAdminsAsync(userManager);

            await SeederAsync.SeedAmenitiesAsync(makanakDbContext);

            await SeederAsync.SeedGovernoraets(makanakDbContext);
        }
    }
}