using FinTrack.API.Application.Interfaces;
using FinTrack.API.Core.Entities;
using FinTrack.API.Infrastructure.Identity.DTO;
using FinTrack.API.Infrastructure.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FinTrack.API.Infrastructure.Identity.Services
{
    public class JwtTokenService : IJwtTokenService
    {

        private IJwtSigningService _signingService;
        private JwtOptions _jwtOptions;

        public JwtTokenService(IJwtSigningService signingService, IOptions<JwtOptions> jwtOptions)
        {
            _signingService = signingService;
            _jwtOptions = jwtOptions.Value;
        }
        
        public async Task<string> GenerateTokenAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));


            var descriptor = new SecurityTokenDescriptor()
            {
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                Expires = DateTime.UtcNow.Add(_jwtOptions.LifeTime),
                Subject = new ClaimsIdentity(claims)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateJwtSecurityToken(descriptor);

            string rawToken = $"{token.EncodedHeader}.{token.EncodedPayload}";
            string signedToken = await _signingService.SignTokenAsync(rawToken);

            return signedToken;
        }
    }
}
