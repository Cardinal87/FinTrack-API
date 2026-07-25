

namespace FinTrack.API.Application.Common
{
    public record AuthResponse(string token, string refreshToken, int expiresIn);
}
