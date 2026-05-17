// ─────────────────────────────────────────────────────────────
// GlobalExceptionMiddleware.cs
//
// This middleware wraps the entire request pipeline.
// If ANY service throws an exception, this catches it and
// returns a clean, user-readable JSON response instead of
// the default ugly HTML error page.
// ─────────────────────────────────────────────────────────────

using AmazeCare.API.DTOS;
using System.Net;
using System.Text.Json;

namespace AmazeCare.API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;    // next middleware in pipeline
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        // This method is called for every HTTP request
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);   // try to pass to the next middleware
            }
            catch (Exception ex)
            {
                // If any exception occurs, log it and return a clean JSON error
                _logger.LogError(ex, "Exception caught: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            // Map different exceptions to HTTP status codes
            int statusCode;
            string message;

            if (ex is KeyNotFoundException)
            {
                statusCode = (int)HttpStatusCode.NotFound;   // 404
                message = ex.Message;
            }
            else if (ex is UnauthorizedAccessException)
            {
                statusCode = (int)HttpStatusCode.Unauthorized;  // 401
                message = ex.Message;
            }
            else if (ex is InvalidOperationException || ex is Exception)
            {
                // For most exceptions thrown in services (e.g. "Email already exists")
                // we return 400 Bad Request with the exception message
                statusCode = (int)HttpStatusCode.BadRequest;   // 400
                message = ex.Message;
            }
            else
            {
                statusCode = (int)HttpStatusCode.InternalServerError;  // 500
                message = "Something went wrong. Please try again later.";
            }

            context.Response.StatusCode = statusCode;

            // Return a clean JSON response
            var response = ApiResponse<string>.Fail(message);
            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}

