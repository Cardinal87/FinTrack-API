

namespace FinTrack.API.Application.Common
{
    public record AuthResponse(string accessToken, string refreshToken, int expiresIn = 900);
}
