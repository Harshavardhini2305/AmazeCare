// ExceptionFilter.cs
//
// This is a custom Action Filter for exception handling.
// It is an alternative approach to middleware for handling
// exceptions specifically inside controllers.
// ─────────────────────────────────────────────────────────────

using AmazeCare.API.DTOS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AmazeCare.API.Filters
{
    // Custom Exception Filter - handles exceptions thrown in controllers
    public class AmazeCareExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<AmazeCareExceptionFilter> _logger;

        public AmazeCareExceptionFilter(ILogger<AmazeCareExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "Exception in controller: {Message}",
                context.Exception.Message);

            // Set the result to a clean JSON error response
            context.Result = new BadRequestObjectResult(
                ApiResponse<string>.Fail(context.Exception.Message)
            );

            // Mark exception as handled so it doesn't bubble up
            context.ExceptionHandled = true;
        }
    }
}
