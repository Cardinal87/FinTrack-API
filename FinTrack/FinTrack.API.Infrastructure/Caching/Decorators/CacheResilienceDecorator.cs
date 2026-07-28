using FinTrack.API.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Registry;

namespace FinTrack.API.Infrastructure.Caching.Decorators
{
    public class CacheResilienceDecorator : ICacheService
    {

        private readonly ICacheService _inner;
        private readonly ResiliencePipeline _cachePipeline;
        private readonly ILogger<CacheResilienceDecorator> _logger;
        public CacheResilienceDecorator(ICacheService inner, ResiliencePipelineProvider<string> provider, ILogger<CacheResilienceDecorator> logger) 
        {
            _inner = inner;
            _cachePipeline = provider.GetPipeline("cache-pipeline");
            _logger = logger;
        }

        public async Task<bool> TrySetKeyOnlyAsync(string key, TimeSpan ttl, CancellationToken ct = default)
        {
            try
            {
                return await _cachePipeline.ExecuteAsync(async ct =>
                {
                    return await _inner.TrySetKeyOnlyAsync(key,ttl, ct);
                }, ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to access cache");
                return false;
            }
        }

        async public Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class
        {
            try
            {
                return await _cachePipeline.ExecuteAsync(async ct =>
                {
                    var result = await _inner.GetAsync<T>(key, ct);
                    return result;
                },
                ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to access cache");
                return null;
            }
        }

        async public Task RemoveByKeyAsync(string key, CancellationToken ct = default)
        {
            try
            {
                await _cachePipeline.ExecuteAsync(async ct =>
                {
                    await _inner.RemoveByKeyAsync(key, ct);
                },
                ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to access cache");
            }
        }

        async public Task RemoveByPatternAsync(string pattern, CancellationToken ct = default)
        {
            try
            {
                await _cachePipeline.ExecuteAsync(async ct =>
                {
                    await _inner.RemoveByKeyAsync(pattern, ct);
                },
                ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to access cache");
            }
        }

        async public Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default) where T : class
        {
            try
            {
                await _cachePipeline.ExecuteAsync(async ct =>
                {
                    await _inner.SetAsync(key, value, ttl, ct);
                }, ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to access cache");
            }
        }

    }
}
