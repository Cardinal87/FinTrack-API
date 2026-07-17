namespace FinTrack.API.Infrastructure.Identity.DTO
{
    public class RefreshToken
    {
        public Guid Id { get; init; }
        public string TokenHash { get; init; } = null!;
        public Guid UserId { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime ExpiresAt { get; init; }
        public bool IsRevoked { get; set; }
        public DateTime? RevokedAt { get; set; }
        public Guid? ReplacedByTokenId { get; set; }
    }
}
