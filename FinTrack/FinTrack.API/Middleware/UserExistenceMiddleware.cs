
using FinTrack.API.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace FinTrack.API.Middleware
{
    public class UserExistenceMiddleware
    {
        private readonly RequestDelegate _next;

        public UserExistenceMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        async public Task InvokeAsync(HttpContext context,
                                      IUserRepository userRepository,
                                      ILogger<UserExistenceMiddleware> logger)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var jti = context.User.FindFirst(t => t.Type == JwtRegisteredClaimNames.Jti)?.Value ?? "undefined";
                var idClaimValue = context.User.Identity.Name;
                if (string.IsNullOrEmpty(idClaimValue))
                {
                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Detail = "Invalid token format"
                    };

                    logger.LogWarning("Attempt to access with incorrect token format without user id. Token id: {Jti}",
                                       jti);

                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(problemDetails);
                    return;
                }

                var isSuccess = Guid.TryParse(idClaimValue, out Guid userId);
                if (!isSuccess)
                {
                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Detail = "Invalid token format"
                    };

                    logger.LogWarning("Incorrect userId format: {Sub}. Token id: {Jti}",
                                       jti,
                                       idClaimValue);

                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(problemDetails);
                    return;
                }


                var user = await userRepository.GetByIdAsync(userId);

                if (user == null)
                {
                    var problemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Detail = "User not found"
                    };

                    logger.LogWarning("Attempt to access by non-existent user with id {UserId}. Token id: {Jti}",
                                           userId,
                                           jti);

                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(problemDetails);
                    return;
                }

            }
            
            await _next(context);
        }
    }
}
