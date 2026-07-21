
using FinTrack.API.Infrastructure.Identity.DTO;

namespace FinTrack.API.Infrastructure.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetTokenByHashAsync(string hash, CancellationToken ct = default);
        Task AddTokenAsync(RefreshToken token, CancellationToken ct = default);
        Task UpdateTokenAsync(RefreshToken token, CancellationToken ct = default);
    }
}
