using CarConfigurator.Business.Services;
using System.Net;
using System.Text.Json;

namespace WebAPI.Middleware
{
    // Unhandled exception appears in controllers/services -> returns a clean HTTP 500 JSON response
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        // LogService is SCOPED but middleware is created ONCE (singleton-like)
        public async Task InvokeAsync(HttpContext context, LogService logService)
        {
            try
            {
                // Run the rest of the pipeline (other middleware + the controller action).
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception processing {Path}", context.Request.Path);

                try
                {
                    await logService.WriteAsync("Error",
                        $"Unhandled exception at {context.Request.Path}: {ex.Message}");
                }
                catch { /* other logging failures */ }

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var payload = JsonSerializer.Serialize(new
                {
                    error = "An unexpected error occurred while processing your request."
                });

                await context.Response.WriteAsync(payload);
            }
        }
    }
}
