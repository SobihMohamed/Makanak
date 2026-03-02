using Makanak.Domain.Models.Identity;
using Makanak.Persistance.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makanak.Persistance.ProgramServices
{
    public static class PersistenceServiceRegisteration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            #region DB Connections
            services.AddDbContext<MakanakDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            // مدام استخدمنا ال IdentityCore 
            // دي افصل من ال Identity 
            // بس بتضيف بقا ال [identiyRole] لوحدها 
            // وبتزود كمان ال [AddDefaultTokenProvider] and [Service.AddDataProtection] 
            //  عشان تشتغل مع ال Roles وال Password Reset و ال Email Confirmation
            services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
            })
             .AddRoles<IdentityRole>() // عشان تشغل الـ Roles
             .AddEntityFrameworkStores<MakanakDbContext>()
             .AddDefaultTokenProviders();

            services.AddDataProtection();
            #endregion
            return services;
        }
    }
}
