using Makanak.Abstraction.IServices.Manager;
using Makanak.Abstraction.IServices.RealTimeNotifier;
using Makanak.Domain.Contracts.InitializerDB;
using Makanak.Domain.Contracts.UOW;
using Makanak.Persistance.Extensions;
using Makanak.Persistance.Hubs;
using Makanak.Persistance.Implements.InitializerImplement;
using Makanak.Persistance.Implements.RealTimeNotifications;
using Makanak.Persistance.Implements.UOW;
using Makanak.Persistance.ProgramServices;
using Makanak.Presentation.Extensions;
using Makanak.Services.AutoMapper.Admin;
using Makanak.Services.AutoMapper.BookingMapper;
using Makanak.Services.AutoMapper.NotificationMapper;
using Makanak.Services.AutoMapper.PropertyMapper;
using Makanak.Services.AutoMapper.Resolver;
using Makanak.Services.AutoMapper.ReviewMapper;
using Makanak.Services.AutoMapper.User;
using Makanak.Services.Services.BackgroundServices;
using Makanak.Services.Services.ManagerImplement;
using Makanak.Shared.Common.Settings;
using Makanak.Web.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

namespace Makanak.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            #region Addded Controllers
            // Add services to the container.
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            }); ;
            #endregion

            #region Added Swagger
            // swagger documentation 
            builder.Services.AddSwaggerDocumentation();
            #endregion

            #region Added AutoMapper
            // auto mapper configuration
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile(new UserProfile());
                cfg.AddProfile(new AdminProfile());
                cfg.AddProfile(new PropertyProfile());
                cfg.AddProfile(new BookingProfile());
                cfg.AddProfile(new ReviewProfile());
                cfg.AddProfile(new NotificationProfile());
            });
            // AutoMapper UrlResolver
            builder.Services.AddTransient(typeof(UrlResolver<,>));
            #endregion

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials() // SignalR
                          .WithOrigins(
                          "http://localhost:4200", // Angular Default
                          "http://localhost:3000", // React Default
                          "http://localhost:5173", // Vite/Vue Default
                          "http://localhost:5500"  // VS Code Live Server (?? ????? HTML/JS ???)
                      );
                });
            });

            #region DB Connections
            builder.Services.AddPersistenceServices(builder.Configuration);
            #endregion

            #region Dependency Injections
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IServiceManager, ServiceManager>();
            builder.Services.AddScoped<IDbInitializer, DbInitialized>();
            builder.Services.AddSignalR();
            builder.Services.AddScoped<IRealTimeNotifier, SignalRNotifier>();

            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("StripeSettings"));
            // background service
            builder.Services.AddHostedService<BookingStatusWorker>();
            #endregion

            #region JWT Configuration
            builder.Services.AddAuthentication(config =>
            {
                config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = false; // no need to string token need data only
                options.RequireHttpsMetadata = false; // no need to be https but in production should be true
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JWTOptions:Issuer"],

                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWTOptions:Audience"],

                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTOptions:SecurityKey"])),


                    ClockSkew = TimeSpan.Zero // to avoid delay in token expiration time
                };
                // Signal-R
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        // 1. ??? ??? ?????? ?? ??? Query String
                        var accessToken = context.Request.Query["access_token"];

                        // 2. ??? ??????? ?? ?? ???? ??? Hub ???????
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/notify"))
                        {
                            // 3. ?? ????? ?? ?????? ?? ??? Query ????? ??? Context ???? ???? Validate
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            #endregion

            #region App 
            var app = builder.Build();

            #region Data Seeding Configuration
            await app.SeedDatabaseAsync();
            #endregion

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerDocumentation();
            }
            app.UseMiddleware<GlobalErrorHandlerMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseStaticFiles();

            app.MapControllers();

            app.MapHub<NotificationHub>("/notify");

            app.Run();
            #endregion
        }
    }
}
