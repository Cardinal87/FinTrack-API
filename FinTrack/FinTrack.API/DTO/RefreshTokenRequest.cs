using System.Text.Json.Serialization;

namespace FinTrack.API.DTO
{
    public class RefreshTokenRequest
    {
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = null!;
    }
}
