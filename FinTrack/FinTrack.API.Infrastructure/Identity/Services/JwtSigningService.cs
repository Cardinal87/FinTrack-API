using FinTrack.API.Infrastructure.Identity.DTO;
using FinTrack.API.Infrastructure.Interfaces;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace FinTrack.API.Infrastructure.Identity.Services
{
    public class JwtSigningService : IJwtSigningService
    {
        private const string rawTokenFormat = @"^[A-Za-z0-9_-]+\.[A-Za-z0-9_-]+$";
        private readonly VaultOptions _options;
        private readonly HttpClient _client;


        public JwtSigningService(IOptions<VaultOptions> options, IHttpClientFactory client)
        {
            _client = client.CreateClient("SigningService");
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

            var response = await _client.PostAsJsonAsync($"v1/transit/sign/{_options.KeyName}", new
            {
                input = base64
            });

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<VaultSign>();

            var sign = result.data.signature.Split(':')[2];

            sign = sign.Replace('+', '-')
                        .Replace('/', '_')
                        .TrimEnd('=');
            return $"{rawToken}.{sign}";
        }

        async public Task<bool> VerifyTokenAsync(string token)
        {
            var sp = token.Split('.');

            var payload = sp[0] + '.' + sp[1];
            var sign = sp[2].Replace('-', '+')
                            .Replace('_', '/');

            var padding = (sign.Length % 4) switch
            {
                2 => "==",
                3 => "=",
                _ => ""
            };
            sign += padding;

            byte[] bytes = Encoding.UTF8.GetBytes(payload);
            string base64 = Convert.ToBase64String(bytes);


            var response = await _client.PostAsJsonAsync($"/v1/transit/verify/{_options.KeyName}", new
            {
                input = base64,
                signature = $"vault:v1:{sign}"
            });
            response.EnsureSuccessStatusCode();


            var result = await response.Content.ReadFromJsonAsync<VaultVerify>();

            if (result.data.valid)
            {
                return true;
            }
            return false;
        }

        readonly record struct VaultSign(VaultSign.Data data)
        {
            public readonly record struct Data(int key_version, string signature);
        }

        readonly record struct VaultVerify(VaultVerify.Data data)
        {
            public readonly record struct Data(bool valid);
        }
    }
}