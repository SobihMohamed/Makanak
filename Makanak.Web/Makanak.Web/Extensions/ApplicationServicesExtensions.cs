using Makanak.Abstraction.IServices.Cashing;
using Makanak.Abstraction.IServices.Manager;
using Makanak.Abstraction.IServices.RealTimeNotifier;
using Makanak.Domain.Contracts.InitializerDB;
using Makanak.Domain.Contracts.UOW;
using Makanak.Persistance.Implements.InitializerImplement;
using Makanak.Persistance.Implements.RealTimeNotifications;
using Makanak.Persistance.Implements.UOW;
using Makanak.Services.AutoMapper.Resolver;
using Makanak.Services.Services.BackgroundServices;
using Makanak.Services.Services.CashingImplement;
using Makanak.Services.Services.ManagerImplement;
using Makanak.Shared.Common.Settings;
using System.Text.Json.Serialization;

namespace Makanak.Web.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            services.AddTransient(typeof(UrlResolver<,>));

            var paymobSection = configuration.GetSection("PaymobSettings");

            services.Configure<PaymobSettings>(paymobSection);

            var paymobSettings = paymobSection.Get<PaymobSettings>();

            // DI & Caching
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddHttpClient("PaymobClient", client =>
            {
                client.BaseAddress = new Uri(paymobSettings!.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            services.AddScoped<IServiceManager, ServiceManager>();
            services.AddScoped<IDbInitializer, DbInitialized>();
            services.AddScoped<IRealTimeNotifier, SignalRNotifier>();

            services.AddMemoryCache();
            services.AddSingleton<ICacheService, MemoryCacheService>();

            // background service
            services.AddHostedService<BookingStatusWorker>();

            return services;
        }
    }
}