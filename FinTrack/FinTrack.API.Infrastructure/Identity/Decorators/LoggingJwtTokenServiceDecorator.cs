using FinTrack.API.Application.Interfaces;
using FinTrack.API.Core.Entities;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;

namespace FinTrack.API.Infrastructure.Identity.Decorators
{
    public class LoggingJwtTokenServiceDecorator : IJwtTokenService
    {
        private readonly IJwtTokenService _innerJwtService;
        private readonly ILogger<LoggingJwtTokenServiceDecorator> _logger;

        public LoggingJwtTokenServiceDecorator(IJwtTokenService innerJwtService, ILogger<LoggingJwtTokenServiceDecorator> logger)
        {
            _innerJwtService = innerJwtService;
            _logger = logger;
        }

        async public Task<string> GenerateTokenAsync(User user)
        {
            var token = await _innerJwtService.GenerateTokenAsync(user);

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var decoded = jwtTokenHandler.ReadJwtToken(token);

            var sub = decoded.Claims.FirstOrDefault(t => t.Type == JwtRegisteredClaimNames.Sub)?.Value;
            var jti = decoded.Claims.FirstOrDefault(t => t.Type == JwtRegisteredClaimNames.Jti)?.Value;

            _logger.LogInformation("User with id {userId} get token with sub {sub} and jti {jti}",
                                    user.Id,
                                    sub ?? "Undefined",
                                    jti ?? "Undefined");

            return token;
        }
    }
}
