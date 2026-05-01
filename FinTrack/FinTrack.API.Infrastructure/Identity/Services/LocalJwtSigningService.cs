using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
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

        async public Task<string> SignTokenAsync(string rawToken)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(rawToken);
            using (var hmac = new HMACSHA256(_key)){
            
                byte[] hash = hmac.ComputeHash(bytes);

                string sign = Base64UrlEncoder.Encode(hash);
                
                return $"{rawToken}.{sign}";
            }
        }

        async public Task<bool> VerifyTokenAsync(string token)
        {   
            var sp = token.Split('.');

            var payload = sp[0] + '.' + sp[1];
            var tokenSign = Base64UrlEncoder.DecodeBytes(sp[2]);

            byte[] bytes = Encoding.UTF8.GetBytes(payload);
            using(var hmac = new HMACSHA256(_key))
            {
                byte[] hash = hmac.ComputeHash(bytes);

                return CryptographicOperations.FixedTimeEquals(tokenSign, hash);
            }
        }
    }
}
