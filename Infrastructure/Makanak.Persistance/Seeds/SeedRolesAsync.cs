using Microsoft.AspNetCore.Identity;
using Makanak.Domain.EnumsHelper.User; // تأكد من الـ Namespace بتاع الـ Enum

namespace Makanak.Persistance.Seeds
{
    public static class RoleSeeder
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            // بنلف على كل الأنواع اللي في الـ Enum بتاعك
            foreach (var role in Enum.GetNames(typeof(UserTypes)))
            {
                // لو الرول مش موجود في الداتابيز.. كارته
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}