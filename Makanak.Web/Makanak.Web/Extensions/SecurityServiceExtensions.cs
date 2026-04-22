using Makanak.Abstraction.IServices.Cashing;
using Makanak.Abstraction.IServices.Manager;
using Makanak.Abstraction.IServices.RealTimeNotifier;
using Makanak.Domain.Contracts.InitializerDB;
using Makanak.Domain.Contracts.UOW;
using Makanak.Persistance.Implements.InitializerImplement;
using Makanak.Persistance.Implements.RealTimeNotifications;
using Makanak.Persistance.Implements.UOW;
using Makanak.Services.AutoMapper.Admin;
using Makanak.Services.AutoMapper.AmenityMapper;
using Makanak.Services.AutoMapper.BookingMapper;
using Makanak.Services.AutoMapper.DisputeMapper;
using Makanak.Services.AutoMapper.GovernorateMapper;
using Makanak.Services.AutoMapper.NotificationMapper;
using Makanak.Services.AutoMapper.PropertyMapper;
using Makanak.Services.AutoMapper.Resolver;
using Makanak.Services.AutoMapper.ReviewMapper;
using Makanak.Services.AutoMapper.User;
using Makanak.Services.Services.BackgroundServices;
using Makanak.Services.Services.CashingImplement;
using Makanak.Services.Services.ManagerImplement;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

namespace Makanak.Web.Extensions
{
    public static class SecurityServiceExtensions
    {
        // added Security & CORS related services for API 
        public static IServiceCollection AddCustomCors(this IServiceCollection services, IConfiguration config)
        {
            var allowedOrigins = config.GetSection("AllowedOrigins")
                .Get<string[]>() ?? Array.Empty<string>();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials() // SignalR
                          .WithOrigins(allowedOrigins);
                });
            });

            return services;
        }

        // 3. added JWT Authentication & Signal-R related services for API
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services,
                IConfiguration config, IWebHostEnvironment env)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = false;

                options.RequireHttpsMetadata = env.IsProduction(); // enforce HTTPS in production

                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = config["JWTOptions:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = config["JWTOptions:Audience"],
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWTOptions:SecurityKey"])),
                    ClockSkew = TimeSpan.Zero
                };

                // SignalR Events
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/notify"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }
    }
}
