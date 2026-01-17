using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models; 

namespace Makanak.Presentation.Extensions
{
    public static class SwaggerServiceExtensions
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(options =>
            {
                // 1. المعلومات الأساسية
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Makanak API",
                    Version = "v1",
                    Description = "API documentation for Makanak application.",
                    Contact = new OpenApiContact
                    {
                        Name = "Makanak Support",
                        Email = "sobihmohamedsobih@gmail.com"
                    }
                });

                // 2. تعريف نظام الأمان (JWT Setup)
                // في الإصدار 6.6.2 بنحط الـ Reference هنا عادي
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter your JWT Token only (without 'Bearer' prefix).",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    }
                };

                options.AddSecurityDefinition("Bearer", securityScheme);

                // 3. تفعيل القفل
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        securityScheme,
                        new List<string>()
                    }
                });
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Makanak API v1"));
            return app;
        }
    }
}