
using FinTrack.API.Infrastructure.Interfaces;
using System.Collections.Concurrent;
using System.Text.Json;

namespace FinTrack.API.TestMocks.Cache
{
    public class CacheServiceMock : ICacheService
    {
        private readonly ConcurrentDictionary<string, CacheEntry> _cache = new();
        private readonly JsonSerializerOptions _serializerOptions;

        public CacheServiceMock()
        {
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
                WriteIndented = false
            };
        }

        public Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default) where T : class
        {
            var json = JsonSerializer.Serialize(value, _serializerOptions);
            var expiration = DateTime.UtcNow.Add(ttl);

            _cache.AddOrUpdate(key, new CacheEntry(json, expiration), (_, _) => new CacheEntry(json, expiration));
            return Task.CompletedTask;
        }

        public Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class
        {
            if (!_cache.TryGetValue(key, out var entry))
                return Task.FromResult<T?>(null);

            if (entry.Expiration < DateTime.UtcNow)
            {
                _cache.TryRemove(key, out _);
                return Task.FromResult<T?>(null);
            }

            var result = JsonSerializer.Deserialize<T>(entry.JsonValue, _serializerOptions);
            return Task.FromResult(result);
        }

        public Task RemoveByKeyAsync(string key, CancellationToken ct = default)
        {
            _cache.TryRemove(key, out _);
            return Task.CompletedTask;
        }

        public Task RemoveByPatternAsync(string pattern, CancellationToken ct = default)
        {
            var keysToRemove = _cache.Keys.Where(k => MatchesPattern(k, pattern)).ToList();
            foreach (var key in keysToRemove)
            {
                _cache.TryRemove(key, out _);
            }
            return Task.CompletedTask;
        }

        private bool MatchesPattern(string key, string pattern)
        {
            var regexPattern = "^" + System.Text.RegularExpressions.Regex.Escape(pattern).Replace("\\*", ".*") + "$";
            return System.Text.RegularExpressions.Regex.IsMatch(key, regexPattern);
        }

        public void Reset()
        {
            _cache.Clear();
        }

        private record CacheEntry(string JsonValue, DateTime Expiration);
    }
}
