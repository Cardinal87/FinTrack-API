
using FinTrack.API.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace FinTrack.API.Infrastructure.Caching.Services
{
    public class RedisCacheService : ICacheService
    {

        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _db;
        private readonly ILogger<RedisCacheService> _logger;
        private readonly JsonSerializerOptions _serializerOptions;
        public RedisCacheService(IConnectionMultiplexer redis, ILogger<RedisCacheService> logger) 
        { 
            _redis = redis;
            _db = _redis.GetDatabase();
            _logger = logger;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
                WriteIndented = false
            };
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default) where T : class
        {
            var json = JsonSerializer.Serialize(value, _serializerOptions);

            if (json == null)
            {
                _logger.LogWarning("Failed to serialize value for key: {key}", key);
                return;
            }
            await _db.StringSetAsync(key, json, ttl);
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class
        {
            var json = await _db.StringGetAsync(key);
            if (json.IsNull)
            {
                return null;
            }

            T? result = JsonSerializer.Deserialize<T>(json!, _serializerOptions);

            if (result == null)
            {
                _logger.LogWarning("Failed to deserialize recieved value for key: {key}", key);
                return null;
            }
            return result;
        }

        public async Task RemoveByKeyAsync(string key, CancellationToken ct = default)
        {
            await _db.KeyDeleteAsync(key);
        }

        public async Task RemoveByPatternAsync(string pattern, CancellationToken ct = default)
        {
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var keys = server.KeysAsync(pattern: pattern);

            await foreach (var key in keys)
            {
                await _db.KeyDeleteAsync(key);
            }
        }

        public async Task<bool> TrySetKeyOnlyAsync(string key, TimeSpan ttl, CancellationToken ct = default)
        {
            return await _db.StringSetAsync(key, 1, ttl, When.NotExists);
        }
        
    }
}
