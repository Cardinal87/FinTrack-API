using FinTrack.API.Application.Common;
using FinTrack.API.Application.Interfaces;
using FinTrack.API.Core.Entities;
using FinTrack.API.Infrastructure.Identity.DTO;
using FinTrack.API.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
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
        private ILogger<JwtTokenService> _logger;

        public JwtTokenService(IJwtSigningService signingService, IOptions<JwtOptions> jwtOptions, ILogger<JwtTokenService> logger)
        {
            _signingService = signingService;
            _jwtOptions = jwtOptions.Value;
            _logger = logger;
        }

        public async Task<TokenGenerationResult> GenerateTokenAsync(User user, bool challenge = false)
        {
            var jti = Guid.NewGuid().ToString();
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, jti)
            };

            if (challenge)
            {
                claims.Add(new Claim("type", "2fa_pending"));
            }
            else
            {
                claims.AddRange(user.Roles.Select(role => new Claim("role", role)));
            }

            var lifeTime = challenge ?
                _jwtOptions.ChallengeTokenLifeTime :
                _jwtOptions.AccessTokenLifeTime;

            var descriptor = new SecurityTokenDescriptor()
            {
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                Expires = DateTime.UtcNow.Add(lifeTime),
                Subject = new ClaimsIdentity(claims)
            };

            var header = new JwtHeader(signingCredentials: null);
            header["alg"] = "ed25519";

            var payload = new JwtPayload(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.Add(lifeTime),
                issuedAt: DateTime.UtcNow
            );

            var token = new JwtSecurityToken(header, payload);

            string rawToken = $"{token.EncodedHeader}.{token.EncodedPayload}";
            string signedToken = await _signingService.SignTokenAsync(rawToken);

            var type = challenge ? "Challenge" : "Access";
            _logger.LogInformation("Token of type {type} with jti {jti} was issued to the user with id {id}",
                                    type,
                                    jti,
                                    user.Id);
            return new TokenGenerationResult(signedToken, (int)lifeTime.TotalSeconds);


        }
    }
}
