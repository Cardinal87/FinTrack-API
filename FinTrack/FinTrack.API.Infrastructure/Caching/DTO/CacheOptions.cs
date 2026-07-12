using FinTrack.API.Infrastructure.Common.DTO;

namespace FinTrack.API.Infrastructure.Caching.DTO
{
    public readonly record struct CacheOptions(TimeSpan ttl, CircuitBreakerOptions cacheBreaker);
}