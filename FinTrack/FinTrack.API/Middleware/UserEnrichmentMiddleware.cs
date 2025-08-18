using Serilog;
using System.IdentityModel.Tokens.Jwt;

namespace FinTrack.API.Middleware
{
    public class UserEnrichmentMiddleware
    {
        private readonly RequestDelegate _next;

        public UserEnrichmentMiddleware(RequestDelegate next)
        {
            _next = next;
        }


        async public Task InvokeAsync(HttpContext context, IDiagnosticContext diagnosticContext)
        {
            var jti = context.User.FindFirst(t => t.Type == JwtRegisteredClaimNames.Jti)?.Value ?? "undefined";
            var userId = context.User.Identity?.Name ?? "undefined";
            diagnosticContext.Set("Token Id", jti);
            diagnosticContext.Set("User Id", userId);
            await _next(context);
        }
    }
}
