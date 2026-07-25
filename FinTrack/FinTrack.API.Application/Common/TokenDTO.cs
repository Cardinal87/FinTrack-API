namespace FinTrack.API.Application.Common
{
    public record struct RefreshTokenRotationResult(bool IsSuccess, string? RefreshToken, Guid UserId);
    public record struct TokenGenerationResult(string Token, int ExpiresIn);
}
