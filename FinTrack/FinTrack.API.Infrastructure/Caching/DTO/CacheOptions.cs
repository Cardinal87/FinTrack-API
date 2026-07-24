using FinTrack.API.Infrastructure.Common.DTO;

namespace FinTrack.API.Infrastructure.Caching.DTO
{
    public class CacheOptions()
    {
        public TimeSpan DefaultTTL { get; set; }
        public CircuitBreakerOptions CircuitBreakerOptions { get; set; } = null!;
    }

}