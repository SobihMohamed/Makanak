
using Makanak.Services.AutoMapper.Admin;
using Makanak.Services.AutoMapper.AmenityMapper;
using Makanak.Services.AutoMapper.BookingMapper;
using Makanak.Services.AutoMapper.DisputeMapper;
using Makanak.Services.AutoMapper.GovernorateMapper;
using Makanak.Services.AutoMapper.NotificationMapper;
using Makanak.Services.AutoMapper.PropertyMapper;
using Makanak.Services.AutoMapper.ReviewMapper;
using Makanak.Services.AutoMapper.User;
using Microsoft.Extensions.DependencyInjection;
namespace SoftBridge.Services.AutoMapper
{
    public static class AutoMapperService
    {
        public static IServiceCollection InjectAutoMapperService(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg =>
            {
                //cfg.AddProfile(new [Auth]Profile());
                cfg.AddProfile(new UserProfile());
                cfg.AddProfile(new AdminProfile());
                cfg.AddProfile(new PropertyProfile());
                cfg.AddProfile(new BookingProfile());
                cfg.AddProfile(new ReviewProfile());
                cfg.AddProfile(new NotificationProfile());
                cfg.AddProfile(new DisputeProfile());
                cfg.AddProfile(new GovernorateProfile());
                cfg.AddProfile(new AmenityProfile());
            });
            return services;
        }
    }
}
