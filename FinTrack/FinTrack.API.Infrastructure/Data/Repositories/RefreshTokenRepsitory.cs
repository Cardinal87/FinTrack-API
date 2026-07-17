using FinTrack.API.Core.Exceptions;
using FinTrack.API.Infrastructure.Identity.DTO;
using FinTrack.API.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.API.Infrastructure.Data.Repositories
{
    public class RefreshTokenRepsitory : IRefreshTokenRepository
    {
        private readonly DatabaseClient _client;

        public RefreshTokenRepsitory(DatabaseClient client)
        {
            _client = client;
        }

        public Task AddTokenAsync(RefreshToken token, CancellationToken ct = default)
        {
            _client.Add(token);
            return Task.CompletedTask;
        }

        public async Task<RefreshToken?> GetTokenByHashAsync(string hash, CancellationToken ct = default)
        {
            var token = await _client.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == hash, ct);
            return token;
        }

        public async Task UpdateTokenAsync(RefreshToken token, CancellationToken ct = default)
        {
            var existingToken = await _client.RefreshTokens.FindAsync(token.Id, ct);
            if (existingToken == null)
            {
                throw new EntityNotFoundException($"Refresh token with id {token.Id} does not exist");
            }
            _client.Update(token);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _client.SaveChangesAsync(ct);
        }
    }
}
