using FinTrack.API.Infrastructure.Identity.DTO;
using FinTrack.API.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace FinTrack.API.Infrastructure.Identity.Services
{
    public class JwtSigningService : IJwtSigningService
    {
        private static Regex rawTokenRegex = new (@"^[A-Za-z0-9_-]+\.[A-Za-z0-9_-]+$", RegexOptions.Compiled);
        private static  Regex signedTokenRegex = new (@"^[A-Za-z0-9_-]+\.[A-Za-z0-9_-]+\.[A-Za-z0-9_-]+$", RegexOptions.Compiled); 
        private readonly VaultOptions _options;
        private readonly HttpClient _client;
        private readonly ILogger<JwtSigningService> _logger;


        public JwtSigningService(IOptions<VaultOptions> options, IHttpClientFactory client, ILogger<JwtSigningService> logger)
        {
            _client = client.CreateClient("SigningService");
            _options = options.Value;
            _logger = logger;
        }



        async public Task<string> SignTokenAsync(string rawToken)
        {
            
            if (!rawTokenRegex.IsMatch(rawToken))
            {
                _logger.LogWarning("Signing rejected: raw token does not match input format: {Pattern}", rawTokenRegex.ToString());
                throw new ArgumentException("Invalid input token format");
            }
            byte[] bytes = Encoding.UTF8.GetBytes(rawToken);
            string base64 = Convert.ToBase64String(bytes);

            var requestUri = $"v1/transit/sign/{_options.KeyName}";
            _logger.LogDebug("Sending sign request to Vault. KeyName: {KeyName}",
                                    _options.KeyName);

            var response = await _client.PostAsJsonAsync(requestUri, new
            {
                input = base64
            });

            await EnsureVaultSuccessAsync(response);

            var result = await response.Content.ReadFromJsonAsync<VaultSign>();

            var sign = result.data.signature.Split(':')[2];

            sign = sign.Replace('+', '-')
                        .Replace('/', '_')
                        .TrimEnd('=');

            _logger.LogDebug("Token signed successfully by Vault. KeyVersion: {KeyVersion}",
                                    result.data.key_version);

            return $"{rawToken}.{sign}";
        }

        async public Task<bool> VerifyTokenAsync(string token)
        {
            var sp = token.Split('.');
            
            if (!signedTokenRegex.IsMatch(token))
            {
                _logger.LogWarning("Signing rejected: signed token does not match JWS format");
                throw new ArgumentException("invalid input token format");
            }

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

            var requestUri = $"/v1/transit/verify/{_options.KeyName}";
            _logger.LogDebug("Sending verify request to Vault. KeyName: {KeyName}",
                                    _options.KeyName);

            var response = await _client.PostAsJsonAsync(requestUri, new
            {
                input = base64,
                signature = $"vault:v1:{sign}"
            });

            await EnsureVaultSuccessAsync(response);

            var result = await response.Content.ReadFromJsonAsync<VaultVerify>();

            if (result.data.valid)
            {
                _logger.LogDebug("Token signature verified successfully. KeyName: {KeyName}", _options.KeyName);
                return true;
            }
            _logger.LogInformation("Token verification failed, Vault RequestId: {RequestId}", GetRequestId(response));
            return false;
        }

        private async Task EnsureVaultSuccessAsync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var parsedErrors = "undefined";
                try
                {
                    var errorObj = await response.Content.ReadFromJsonAsync<VaultErrors>();
                    if (errorObj.errors != null)
                    {
                        parsedErrors = string.Join("; ", errorObj.errors);
                    }
                }
                catch
                {
                    parsedErrors = "Failed to parse Vault errors";
                }
                var endpoint = response.RequestMessage?.RequestUri?.ToString() ?? "unknown";

                _logger.LogError(
                    "Vault request with id {RequestId} failed, . StatusCode: {StatusCode}, Reason: {ReasonPhrase}, " +
                    "KeyName: {KeyName}, Endpoint: {Endpoint}, VaultErrors: {Errors}",
                    GetRequestId(response),
                    (int)response.StatusCode,
                    response.ReasonPhrase,
                    _options.KeyName,
                    endpoint,
                    parsedErrors);

                response.EnsureSuccessStatusCode();
            }
        }

        private string GetRequestId(HttpResponseMessage response)
        {
            if (response.Headers.TryGetValues("X-Vault-Request-Id", out var values))
            {
                return values.FirstOrDefault() ?? "undefined";
            }
            return "undefined";
        }

        readonly record struct VaultSign(VaultSign.Data data)
        {
            public readonly record struct Data(int key_version, string signature);
        }

        readonly record struct VaultVerify(VaultVerify.Data data)
        {
            public readonly record struct Data(bool valid);
        }
        readonly record struct VaultErrors(List<string> errors);
    }
}