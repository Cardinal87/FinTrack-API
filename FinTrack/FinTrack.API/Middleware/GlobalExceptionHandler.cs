using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.API.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        
        async public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Unhandled exception. UserId {UserId}",
                httpContext.User.Identity?.Name);

            var problemDetail = new ProblemDetails
            {
                Status = 500,
                Detail = "Unexpected server error"
            };

            await httpContext.Response.WriteAsJsonAsync(problemDetail);
            return true;
        }
    }
}
