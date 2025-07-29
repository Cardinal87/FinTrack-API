using FinTrack.API.Application.Interfaces;
using FinTrack.API.Core.Entities;
using FinTrack.API.Infrastructure.Identity.DTO;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FinTrack.API.Infrastructure.Identity.Services
{
    public class JwtTokenService : IJwtTokenService
    {

        private JwtKeyService _keyService;
        private JwtOptions _jwtOptions;

        public JwtTokenService(JwtKeyService keyService, IOptions<JwtOptions> jwtOptions)
        {
            _keyService = keyService;
            _jwtOptions = jwtOptions.Value;
        }
        
        public string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(_keyService.GetKey());

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
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
                Subject = new ClaimsIdentity(claims)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(descriptor);

            return tokenHandler.WriteToken(token);



        }
    }
}
