using System.Text.Json.Serialization;

namespace FinTrack.API.DTO
{
    public readonly record struct LoginRequest(string Password, string Login);
    public readonly record struct RefreshTokenRequest([property: JsonPropertyName("refresh_token")] string RefreshToken);
    public readonly record struct VerifyCodeRequest(string Code);
}
