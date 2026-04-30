using Makanak.Domain.Models.Identity;
using Makanak.Persistance.Contexts;
using Makanak.Shared.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;

namespace Makanak.Web.Extensions
{
   public static class IdentityServiceExtensions
   {
        public static IServiceCollection InjectIdentityCore(this IServiceCollection services)
            {
            // Add Identity services
            // Add Identity services [Password Security using Salt and Hashing PBKDF2 & HMAC-SHA256]
            services.AddIdentityCore<ApplicationUser>(options =>
                {
                    options.Password.RequireDigit = true;  
                    options.Password.RequireLowercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequiredLength = 6;
                    // for forget password 
                    // to van used => var otp = await _userManager.GeneratePasswordResetTokenAsync(user);
                    options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider; // email provider producr  => 6 otp
                    options.Tokens.ChangeEmailTokenProvider = TokenOptions.DefaultEmailProvider;
                })
                     .AddRoles<IdentityRole>()
                     .AddEntityFrameworkStores<MakanakDbContext>()
                     .AddDefaultTokenProviders();
               
                services.Configure<DataProtectionTokenProviderOptions>(options =>
                {
                    options.TokenLifespan = TimeSpan.FromMinutes(5);
                });
                return services;
            }
        public static IServiceCollection InjectRateLimiting(this IServiceCollection services)
    {
        // Add rate limiting services
        services.AddRateLimiter(options =>
        {
            // Prevent Dos attacks by rejecting requests that exceed the limit
            options.AddFixedWindowLimiter("OtpPolicy", opt =>
            {
                // 2 min => 3 request 
                opt.Window = TimeSpan.FromMinutes(2); // time to expire is 2 minutes 
                opt.PermitLimit = 3; // numbers to try is 3 times
                opt.QueueLimit = 0; // no queue, requests will be rejected immediately when the limit is reached
            });
            // Set the status code for rejected requests to 429 (Too Many Requests)
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests; // too many requests
            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.ContentType = "application/json";

                var response = new ApiResponse<string>(
                    null,
                    "Maximum attempts reached. Please try again after 2 minutes.",
                    429);

                await context.HttpContext.Response.WriteAsJsonAsync(response, cancellationToken: token);
            };
        });
        return services;
    }
    }
}
