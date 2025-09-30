using System.Net;
using System.Text.Json;
using SurveyManagement.Domain.Exceptions;

namespace SurveyManagement.API.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            // Default values
            HttpStatusCode status = HttpStatusCode.InternalServerError;
            string message = "An unexpected error occurred.";

            // Map known exceptions to proper HTTP status codes
            switch (exception)
            {
                case NotFoundException:
                    status = HttpStatusCode.NotFound;
                    message = exception.Message;
                    break;
                case BadRequestException:
                    status = HttpStatusCode.BadRequest;
                    message = exception.Message;
                    break;
                case UnauthorizedException:
                    status = HttpStatusCode.Unauthorized;
                    message = exception.Message;
                    break;
                case ConflictException:
                    status = HttpStatusCode.Conflict;
                    message = exception.Message;
                    break;
                default:
                    message = exception.Message; // For unexpected exceptions
                    break;
            }

            context.Response.StatusCode = (int)status;

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = message,
                Exception = exception.GetType().Name,
                Timestamp = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(json);
        }
    }

    public static class GlobalExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionMiddleware>();
        }
    }
}
