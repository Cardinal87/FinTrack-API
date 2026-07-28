

namespace FinTrack.API.Application.Interfaces
{
    public interface IChallengeTokenTracker
    {
        Task<bool> TryMarkAsUsedAsync(Guid jti, CancellationToken ct = default);
    }
}
