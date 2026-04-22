using Makanak.Persistance.Extensions;
using Makanak.Persistance.Hubs;
using Makanak.Persistance.ProgramServices;
using Makanak.Presentation.Extensions;
using Makanak.Shared.Common.Settings;
using Makanak.Web.Extensions;
using Makanak.Web.Middleware;
using SoftBridge.Services.AutoMapper;

var builder = WebApplication.CreateBuilder(args);

// 1. Database & Infrastructure
builder.Services.InjectDatabaseService(builder.Configuration);

// Add Identity Services & security services 
builder.Services.InjectIdentityCore();
builder.Services.InjectRateLimiting();
// Custom Extensions (Security & CORS)
builder.Services.AddJwtAuthentication(builder.Configuration, builder.Environment);
builder.Services.AddCustomCors(builder.Configuration);

// Add Services to the Container 
builder.Services.AddApplicationServices();
builder.Services.InjectAutoMapperService();

builder.Services.AddSwaggerDocumentation();

builder.Services.AddSignalR();
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("StripeSettings"));



// Build the Application
var app = builder.Build();

// Data Seeding Configuration
await app.SeedDatabaseAsync();


// Configure the HTTP Request Pipeline

// 1.Swagger Tester

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerDocumentation();
    // when deploying to production, put the UseSwaggerDocumentation() inside
    // the if block and remove it from the top of the pipeline configuration
}
else
{
    // 2.used for production environment to enforce HTTPS and HSTS
    app.UseHsts();
}

app.UseMiddleware<GlobalErrorHandlerMiddleware>();
app.UseHttpsRedirection();
app.UseRouting();

// CORS MUST be between UseRouting and UseAuth
app.UseCors("CorsPolicy");

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.MapControllers();
app.MapHub<NotificationHub>("/notify");

app.Run();