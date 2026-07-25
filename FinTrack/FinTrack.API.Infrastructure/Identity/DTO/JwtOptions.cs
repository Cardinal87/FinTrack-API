
namespace FinTrack.API.Infrastructure.Identity.DTO
{
    public class JwtOptions
    {

        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public TimeSpan AccessTokenLifeTime { get; set; }
        public TimeSpan ChallengeTokenLifeTime { get; set; }
        public TimeSpan RefreshTokenLifeTime { get; set; }
    }
}
