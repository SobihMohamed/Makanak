using Makanak.Domain.Exceptions;
using Makanak.Shared.Responses;
using System.Text.Json;

namespace Makanak.Web.Middleware
{
    public class GlobalErrorHandlerMiddleware(RequestDelegate next, ILogger<GlobalErrorHandlerMiddleware> logger, IWebHostEnvironment env)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
                if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
                {
                    await HandleResponseAsync(context, 404, "The requested resource was not found.");
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error : {ex.Message}");
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleResponseAsync(HttpContext context, int statusCode, string msg)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            var resp = new ApiResponse<string>(msg, statusCode);
            resp.IsSuccess = false;
            var json = JsonSerializer.Serialize(resp, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            await context.Response.WriteAsync(json);
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {

            context.Response.ContentType = "application/json";

            string serverErrorMessage = env.IsDevelopment()
                ? $"{exception.Message} \n {exception.StackTrace}" 
                : "An unexpected error occurred on the server. Please try again later."; 

            var resp = exception switch
            {
                NotFoundException_Base => new ApiResponse<string>(exception.Message, 404),
                UnauthorizedException => new ApiResponse<string>(exception.Message, 401),
                BadRequestException BR => new ApiResponse<string>(BR.Message, 400, BR._errors?.ToList()),
                _ => new ApiResponse<string>(serverErrorMessage, 500) 
            };

            context.Response.StatusCode = resp.StatusCode;
            resp.IsSuccess = false;

            var json = JsonSerializer.Serialize(resp, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            await context.Response.WriteAsync(json);

        }
    }
}
