using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace Makanak.Web.Extensions
{
    public static class SecurityServiceExtensions
    {
        // added Security & CORS related services for API 
        public static IServiceCollection AddCustomCors(this IServiceCollection services, IConfiguration config)
        {
            // 1. Get the allowed origins from configuration
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
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;// Set the default authentication scheme to JWT Bearer not cookie-based authentication
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // if unAuthorized, challenge the user to authenticate using JWT Bearer
            }).AddJwtBearer(options =>
            {
                options.SaveToken = false; // do not save the token in the AuthenticationProperties after a successful authentication

                options.RequireHttpsMetadata = env.IsProduction(); // enforce HTTPS in production prevent Man-in-the-Middle (MitM) Attacks
                // 4 test the JWT token validation parameters 
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true, // Validate the issuer of the token
                    ValidIssuer = config["JWTOptions:Issuer"], // The expected issuer of the token, as specified in the configuration
                    
                    ValidateAudience = true, // Validate the audience of the token
                    ValidAudience = config["JWTOptions:Audience"], // The expected audience of the token, as specified in the configuration
                    
                    ValidateLifetime = true, // Validate the token's expiration
                    
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWTOptions:SecurityKey"])), // The key used to sign the token
                    ClockSkew = TimeSpan.Zero // No tolerance for the token expiration no 5 min after expires
                };
                // 5. added Signal-R support for JWT token in query string
                // problem is the JWT token is not sent in the Authorization header for WebSocket connections, so we need to extract it from the query string
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
