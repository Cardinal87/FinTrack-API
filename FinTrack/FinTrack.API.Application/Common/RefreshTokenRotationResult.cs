

namespace FinTrack.API.Application.Common
{
    public record RefreshTokenRotationResult(bool isSuccess, string? refreshToken, Guid userId);
}
