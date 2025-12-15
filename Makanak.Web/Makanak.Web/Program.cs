using Makanak.Abstraction.IServices;
using Makanak.Persistance.ProgramServices;
using Makanak.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security;
using System.Text;

namespace Makanak.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

            #region DB Connections
            builder.Services.AddPersistenceServices(builder.Configuration);
            #endregion

            #region Dependency Injections
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IAttachementServices, AttachementServices>();
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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTOptions:SecurityKey"]))
                
                    ClockSkew = TimeSpan.Zero // to avoid delay in token expiration time
                };
            });
            #endregion
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
