using Makanak.Domain.Identity.Models;
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

            services.AddIdentityCore<ApplicationUser>()
             .AddRoles<IdentityRole>() // عشان تشغل الـ Roles
             .AddEntityFrameworkStores<MakanakDbContext>()
             .AddDefaultTokenProviders();

            services.AddDataProtection();
            #endregion
            return services;
        }
    }
}
