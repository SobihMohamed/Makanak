using Makanak.Abstraction.IServices;
using Makanak.Abstraction.IServices.Admin;
using Makanak.Abstraction.IServices.Auth;
using Makanak.Domain.Contracts.Repos;
using Makanak.Domain.Contracts.UOW;
using Makanak.Persistance.Extensions;
using Makanak.Persistance.Implements.ReposImplement;
using Makanak.Persistance.Implements.UOW;
using Makanak.Persistance.ProgramServices;
using Makanak.Presentation.Extensions;
using Makanak.Services.AutoMapper;
using Makanak.Services.AutoMapper.Resolver;
using Makanak.Services.Services;
using Makanak.Services.Services.Admin;
using Makanak.Services.Services.Auth;
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

            // Add services to the container.

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            }); ;

            // swagger documentation 
            builder.Services.AddSwaggerDocumentation();

            // auto mapper configuration
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile(new UserProfile());
                cfg.AddProfile(new AdminProfile());
            });
            #region DB Connections
            builder.Services.AddPersistenceServices(builder.Configuration);
            #endregion

            #region Dependency Injections
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IAttachementServices, AttachementServices>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IEmailService, EmailServices>();
            builder.Services.AddScoped<IAdminServices, AdminServices>();
            // AutoMapper UrlResolver
            builder.Services.AddTransient(typeof(UrlResolver<,>));
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
        }
    }
}
