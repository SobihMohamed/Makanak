using Makanak.Persistance.Extensions;
using Makanak.Persistance.Hubs;
using Makanak.Persistance.ProgramServices;
using Makanak.Presentation.Extensions;
using Makanak.Shared.Common.Settings;
using Makanak.Web.Extensions;
using Makanak.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add Services to the Container
builder.Services.AddApplicationCoreServices();
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddSwaggerDocumentation();
builder.Services.AddSignalR();
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("StripeSettings"));

// Custom Extensions (Security & CORS)
builder.Services.AddCustomCors(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration, builder.Environment);


// Build the Application
var app = builder.Build();

// Data Seeding Configuration
await app.SeedDatabaseAsync();


// Configure the HTTP Request Pipeline

// 1.Swagger Tester

if (app.Environment.IsDevelopment())
{
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

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.MapControllers();
app.MapHub<NotificationHub>("/notify");

app.Run();