using Makanak.Domain.EnumsHelper.User; 
using Makanak.Domain.Models.PropertyEntities;
using Makanak.Persistance.Contexts;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Makanak.Domain.Models.LocationEntities;
using Makanak.Domain.Exceptions.NotFound;

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

        public static async Task SeedPropertiesAsync(MakanakDbContext context)
        {
            if (!await context.Set<Property>().AnyAsync())
            {
                var firstUser = await context.Users.FirstOrDefaultAsync();
                if (firstUser == null) return;

                var buildPath = AppDomain.CurrentDomain.BaseDirectory;
                var filePath = Path.Combine(buildPath, "Data", "Seeds", "properties.json");

                if (File.Exists(filePath))
                {
                    var propertiesData = await File.ReadAllTextAsync(filePath);
                    var properties = JsonSerializer.Deserialize<List<Property>>(propertiesData);

                    if (properties?.Count > 0)
                    {
                        foreach (var prop in properties)
                        {
                            // ربط العقار باليوزر الموجود
                            prop.OwnerId = firstUser.Id;

                            // بيانات Audit
                            prop.CreatedBy = "Seeder";

                            prop.MainImageUrl = "uploads/Default_Image.png";
                        }

                        await context.Set<Property>().AddRangeAsync(properties);
                        await context.SaveChangesAsync();
                    }
                }
                else
                {
                    throw new FileNotFound(filePath);
                }
            }
        }
    }
}