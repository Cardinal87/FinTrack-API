using FinTrack.API.Infrastructure.Common.DTO;

namespace FinTrack.API.Infrastructure.Caching.DTO
{
    public class CacheOptions()
    {
        public TimeSpan TTL { get; set; }
        public string EndPoint { get; set; } = null!;
        public CircuitBreakerOptions CircuitBreakerOptions { get; set; } = null!;
    }

}