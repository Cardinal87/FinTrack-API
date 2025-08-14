
using FinTrack.API.Application.Interfaces;
using FinTrack.API.Core.Entities;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;

namespace FinTrack.API.Infrastructure.Decorators
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

        public string GenerateToken(User user)
        {
            var token = _innerJwtService.GenerateToken(user);

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var decoded = jwtTokenHandler.ReadJwtToken(token);

            var sub = decoded.Claims.FirstOrDefault(t => t.Type == JwtRegisteredClaimNames.Sub);
            var jti = decoded.Claims.FirstOrDefault(t => t.Type == JwtRegisteredClaimNames.Jti);

            _logger.LogInformation("User with id {userId} get token with sub {Sub} and jti {Jti}",
                                    user.Id,
                                    sub,
                                    jti);

            return token;
        }
    }
}
