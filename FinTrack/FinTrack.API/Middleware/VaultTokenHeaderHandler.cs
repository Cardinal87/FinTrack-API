using FinTrack.API.Infrastructure.Interfaces;

namespace FinTrack.API.Middleware
{
    public class VaultTokenHeaderHandler : DelegatingHandler
    {
        private readonly IVaultTokenProvider _tokenProvider;

        public VaultTokenHeaderHandler(IVaultTokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        async protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Remove("X-Vault-Token");

            var token = await _tokenProvider.GetVaultToken();
            request.Headers.Add("X-Vault-Token", token);
            
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
