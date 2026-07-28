

using FinTrack.API.Application.Interfaces;
using FinTrack.API.Infrastructure.Identity.DTO;
using FinTrack.API.Infrastructure.Interfaces;
using Microsoft.Extensions.Options;

namespace FinTrack.API.Infrastructure.Identity.Services
{
    public class ChallengeTokenTracker : IChallengeTokenTracker
    {
        private readonly JwtOptions _jwtOptions;
        private readonly ICacheKeyProvider _cacheKeyProvider;
        private readonly ICacheService _cacheService;

        public ChallengeTokenTracker(IOptions<JwtOptions> jwtOptions, ICacheKeyProvider cacheKeyProvider, ICacheService cacheService)
        {
            _jwtOptions = jwtOptions.Value;
            _cacheKeyProvider = cacheKeyProvider;
            _cacheService = cacheService;
        }

        public async Task<bool> TryMarkAsUsedAsync(Guid jti, CancellationToken ct = default)
        {
            var key = _cacheKeyProvider.ChallengeToken(jti.ToString());
            return await _cacheService.TrySetKeyOnlyAsync(key, _jwtOptions.ChallengeTokenLifeTime, ct);
        }
    }
}
