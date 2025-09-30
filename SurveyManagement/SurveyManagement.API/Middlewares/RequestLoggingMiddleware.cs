using Microsoft.AspNetCore.Http;
using Serilog;
using System.Diagnostics;

namespace SurveyManagement.API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var request = context.Request;

            Log.Information("Incoming Request: {Method} {Path}", request.Method, request.Path);

            try
            {
                await _next(context);

                stopwatch.Stop();
                Log.Information("Response {StatusCode} for {Method} {Path} in {ElapsedMilliseconds}ms",
                    context.Response.StatusCode,
                    request.Method,
                    request.Path,
                    stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Log.Error(ex, "Exception thrown for {Method} {Path} after {ElapsedMilliseconds}ms",
                    request.Method,
                    request.Path,
                    stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }

    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}
