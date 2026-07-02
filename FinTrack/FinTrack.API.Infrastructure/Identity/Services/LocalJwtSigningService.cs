using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FinTrack.API.Infrastructure.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace FinTrack.API.Infrastructure.Identity.Services
{
    /// <summary>
    /// WARNING: Use this implementation ONLY in Develop or Testing environment
    /// </summary>
    public class LocalJwtSigningService : IJwtSigningService
    {
        private byte[] _key;

        public LocalJwtSigningService()
        {
            GenerateKey();
        }


        [MemberNotNull(nameof(_key))]
        public void GenerateKey()
        {
            var key = new byte[64];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(key);
            _key = key;
        }

        public Task<string> SignTokenAsync(string rawToken)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(rawToken);
            using (var hmac = new HMACSHA256(_key)){
            
                byte[] hash = hmac.ComputeHash(bytes);

                string sign = Base64UrlEncoder.Encode(hash);
                var signedToken = $"{rawToken}.{sign}";

                return Task.FromResult(signedToken);
            }
        }

        public Task<bool> VerifyTokenAsync(string token)
        {
            token = token.Trim();
            var sp = token.Split('.');

            var payload = sp[0] + '.' + sp[1];
            var tokenSign = Base64UrlEncoder.DecodeBytes(sp[2]);

            byte[] bytes = Encoding.UTF8.GetBytes(payload);
            using(var hmac = new HMACSHA256(_key))
            {
                byte[] hash = hmac.ComputeHash(bytes);
                var valid = CryptographicOperations.FixedTimeEquals(tokenSign, hash);

                return Task.FromResult(valid);
            }
        }

        
    }
}
