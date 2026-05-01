using FinTrack.API.Infrastructure.Interfaces;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using FinTrack.API.Infrastructure.Identity.DTO;
using VaultSharp;
using VaultSharp.V1.SecretsEngines.Transit;
using System.Text;

namespace FinTrack.API.Infrastructure.Identity.Services
{
    public class JwtSigningService : IJwtSigningService
    {
        private const string rawTokenFormat = @"^[A-Za-z0-9_-]+\.[A-Za-z0-9_-]+$";
        private readonly VaultOptions _options;
        private readonly IVaultClient _client;


        public JwtSigningService(IOptions<VaultOptions> options, IVaultClient client)
        {
            _client = client;
            _options = options.Value;
        }



        async public Task<string> SignTokenAsync(string rawToken)
        {
            bool b = Regex.IsMatch(rawToken, rawTokenFormat);
            if (!b)
            {
                throw new ArgumentException("invalid input token format");
            }
            
            byte[] bytes = Encoding.UTF8.GetBytes(rawToken);
            string base64 = Convert.ToBase64String(bytes);

            var signOptions = new SignRequestOptions
            {
                Base64EncodedInput = base64,
                MarshalingAlgorithm = MarshalingAlgorithm.jws
            };
            var signResp = await _client.V1.Secrets.Transit.SignDataAsync(_options.KeyName, signOptions);

            var sign = signResp.Data.Signature.Split(':')[2];

            return $"{rawToken}.{sign}";
        }

        async public Task<bool> VerifyTokenAsync(string token)
        {
            var sp = token.Split('.');

            var payload = sp[0] + '.' + sp[1];
            var sign = sp[2];

            byte[] bytes = Encoding.UTF8.GetBytes(payload);
            string base64 = Convert.ToBase64String(bytes);


            var verifyOptions = new VerifyRequestOptions
            {
                Base64EncodedInput = base64,
                Signature = $"vault:v1:{sign}",
                MarshalingAlgorithm = MarshalingAlgorithm.jws,
            };

            var verifyResp = await _client.V1.Secrets.Transit.VerifySignedDataAsync(_options.KeyName, verifyOptions);
            if (verifyResp.Data.Valid)
            {
                return true;
            }

            return false;
        }
    }
}