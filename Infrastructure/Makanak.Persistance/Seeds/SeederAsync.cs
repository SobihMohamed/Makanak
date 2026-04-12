using Makanak.Domain.EnumsHelper.User; 
using Makanak.Domain.Exceptions.NotFound;
using Makanak.Domain.Models.Identity;
using Makanak.Domain.Models.LocationEntities;
using Makanak.Domain.Models.PropertyEntities;
using Makanak.Persistance.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Makanak.Persistance.Seeds
{
    public static class SeederAsync
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in Enum.GetNames(typeof(UserTypes)))
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        public static async Task SeedAdminsAsync(UserManager<ApplicationUser> userManager)
        {
            string adminRole = UserTypes.Admin.ToString();

            var admins = await userManager.GetUsersInRoleAsync(adminRole);

            if (!admins.Any())
            {
                var adminUsers = new List<ApplicationUser>
        {
            new ApplicationUser
            {
                UserStatus = UserStatus.Active,
                DateOfBirth = new DateTime(2000, 1, 1),
                UserType = UserTypes.Admin,
                Name = "System Admin",
                PhoneNumber = "01225869788",
                UserName = "ADMIN@MAKANAK.SITE",
                Email = "admin@makanak.site",
                EmailConfirmed = true,
            }
        };

                string defaultPassword = "AdminPassword@123";

                foreach (var admin in adminUsers)
                {
                    var result = await userManager.CreateAsync(admin, defaultPassword);

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, adminRole);
                    }
                }
            }
        }
        public static async Task SeedGovernoraets(MakanakDbContext context)
        {
            if (!await context.Set<Governorate>().AnyAsync())
            {
                var buildPath = AppDomain.CurrentDomain.BaseDirectory;
                var filePath = Path.Combine(buildPath, "Data", "Seeds", "governorates.json");
                if (File.Exists(filePath))
                {
                    var governoratesData = await File.ReadAllTextAsync(filePath);
                    var governorates = JsonSerializer.Deserialize<List<Governorate>>(governoratesData);
                    if (governorates?.Count > 0)
                    {
                        await context.Set<Governorate>().AddRangeAsync(governorates);
                        await context.SaveChangesAsync();
                    }
                }
                else
                {
                    throw new FileNotFound(filePath);
                }
            }
        }

        public static async Task SeedAmenitiesAsync(MakanakDbContext context)
        {
            
            if (!await context.Set<Amenity>().AnyAsync())
            {
                var buildPath = AppDomain.CurrentDomain.BaseDirectory;
                var filePath = Path.Combine(buildPath, "Data", "Seeds", "amenities.json");

                if (File.Exists(filePath))
                {
                    var amenitiesData = await File.ReadAllTextAsync(filePath);
                    var amenities = JsonSerializer.Deserialize<List<Amenity>>(amenitiesData);

                    if (amenities?.Count > 0)
                    {
                        await context.Set<Amenity>().AddRangeAsync(amenities);
                        await context.SaveChangesAsync();
                    }
                }
            }
        }
    }
}