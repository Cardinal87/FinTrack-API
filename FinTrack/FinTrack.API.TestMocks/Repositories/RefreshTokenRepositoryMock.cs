

using FinTrack.API.Core.Entities;
using FinTrack.API.Infrastructure.Identity.DTO;
using FinTrack.API.Infrastructure.Interfaces;

namespace FinTrack.API.TestMocks.Repositories
{
    public class RefreshTokenRepositoryMock : IRefreshTokenRepository
    {
        private readonly List<RefreshToken> _refreshTokens = [];
        public Task AddTokenAsync(RefreshToken token, CancellationToken ct = default)
        {
            _refreshTokens.Add(token);
            return Task.CompletedTask;
        }

        public Task<RefreshToken?> GetTokenByHashAsync(string hash, CancellationToken ct = default)
        {
            var token = _refreshTokens.FirstOrDefault(x => x.TokenHash == hash);
            return Task.FromResult(token);
        }

        public Task UpdateTokenAsync(RefreshToken token, CancellationToken ct = default)
        {
            return Task.CompletedTask;
        }

        public void Reset()
        {
            _refreshTokens.Clear();
        }
    }
}
