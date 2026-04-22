
using Makanak.Persistance.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Persistance.ProgramServices
{
    public static class AddDbServices
    {
        public static IServiceCollection InjectDatabaseService(this IServiceCollection services, IConfiguration configuration)
        {
            // Add DbContext
            services.AddDbContext<MakanakDbContext>(options =>
               options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            return services;
        }
    }
}
