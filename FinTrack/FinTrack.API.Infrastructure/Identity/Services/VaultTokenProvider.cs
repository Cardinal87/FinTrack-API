
using FinTrack.API.Infrastructure.Identity.DTO;
using FinTrack.API.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data;
using System.Net.Http.Json;

namespace FinTrack.API.Infrastructure.Identity.Services
{
    public class VaultTokenProvider : IVaultTokenProvider
    {
        private VaultOptions _options;
        private SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private string? _vaultToken;
        private DateTime _expires = DateTime.MinValue;
        private HttpClient _client;
        private ILogger<VaultTokenProvider> _logger;

        public VaultTokenProvider(IOptions<VaultOptions> options, IHttpClientFactory client, ILogger<VaultTokenProvider> logger)
        {
            _options = options.Value;
            _client = client.CreateClient("VaultTokenProvider");
            _logger = logger;
        }

        async public Task<string?> GetVaultToken()
        {
            if (_vaultToken != null && DateTime.Now.AddMinutes(5) < _expires)
            {
                return _vaultToken;
            }

            await _semaphore.WaitAsync();
            try
            {
                if (_vaultToken != null && DateTime.Now.AddMinutes(5) < _expires)
                {
                    return _vaultToken;
                }

                var response = await _client.PostAsJsonAsync("/v1/auth/approle/login", new { role_id = _options.RoleID });

                
                var result = await response.Content.ReadFromJsonAsync<VaultResponse>();

                _vaultToken = result.auth.client_token;
                _expires = DateTime.Now.AddSeconds(result.auth.lease_duration);
                _logger.LogInformation("token recieved successfully and expires at {0}", _expires);

                return _vaultToken;

            }
            finally
            {
                _semaphore.Release();
            }
        }

        readonly record struct VaultResponse(VaultResponse.Auth auth)
        {
            public readonly record struct Auth(string client_token, int lease_duration);
        }
    }
}
