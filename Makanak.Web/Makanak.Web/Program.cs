using Makanak.Abstraction.IServices;
using Makanak.Abstraction.IServices.Admin;
using Makanak.Abstraction.IServices.Auth;
using Makanak.Abstraction.IServices.Manager;
using Makanak.Domain.Contracts.InitializerDB;
using Makanak.Domain.Contracts.Repos;
using Makanak.Domain.Contracts.UOW;
using Makanak.Persistance.Extensions;
using Makanak.Persistance.Implements.InitializerImplement;
using Makanak.Persistance.Implements.ReposImplement;
using Makanak.Persistance.Implements.UOW;
using Makanak.Persistance.ProgramServices;
using Makanak.Presentation.Extensions;
using Makanak.Services.AutoMapper;
using Makanak.Services.AutoMapper.Admin;
using Makanak.Services.AutoMapper.BookingMapper;
using Makanak.Services.AutoMapper.PropertyMapper;
using Makanak.Services.AutoMapper.Resolver;
using Makanak.Services.AutoMapper.User;
using Makanak.Services.Services;
using Makanak.Services.Services.Admin;
using Makanak.Services.Services.Auth;
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
            });
            // AutoMapper UrlResolver
            builder.Services.AddTransient(typeof(UrlResolver<,>));
            #endregion

            #region DB Connections
            builder.Services.AddPersistenceServices(builder.Configuration);
            #endregion

            #region Dependency Injections
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IServiceManager, ServiceManager>();
            builder.Services.AddScoped<IDbInitializer, DbInitialized>();
            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("StripeSettings"));
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

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseStaticFiles();

            app.MapControllers();

            app.Run();
            #endregion
        }
    }
}
