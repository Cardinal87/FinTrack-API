using FinTrack.API.Application.Interfaces;
using FinTrack.API.Core.Exceptions;
using FinTrack.API.Infrastructure.Caching.DTO;
using FinTrack.API.Infrastructure.Data;
using FinTrack.API.Infrastructure.Identity.DTO;
using FinTrack.API.Infrastructure.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text.Json;

namespace FinTrack.API.Infrastructure.Identity.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ICacheService _cache;
        private readonly ICacheKeyProvider _provider;
        private readonly ILogger<RefreshTokenService> _logger;
        private readonly JwtOptions _jwtOptions;
        private readonly CacheOptions _cacheOptions;

        public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository, 
            ICacheService cache, 
            ICacheKeyProvider provider, 
            ILogger<RefreshTokenService> logger, 
            IOptions<JwtOptions> jwtOptions,
            IOptions<CacheOptions> cacheOptions)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _cache = cache;
            _provider = provider;
            _logger = logger;
            _jwtOptions = jwtOptions.Value;
            _cacheOptions = cacheOptions.Value;
        }

        public async Task<string> GenerateRefreshTokenAsync(Guid userId, CancellationToken ct = default)
        {
            var token = GenerateToken();

            var record = new RefreshToken
            {
                Id = Guid.NewGuid(),
                TokenHash = token.Hash,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.Add(_jwtOptions.RefreshTokenLifeTime),
                IsRevoked = false
            };

            _logger.LogDebug("saving refresh token record to database");
            await _refreshTokenRepository.AddTokenAsync(record, ct);

            _logger.LogDebug("saving refresh token to cache");
            var cacheEntry = new CacheEntry(userId, record.ExpiresAt, false);
            await _cache.SetAsync(_provider.RefreshToken(token.Hash), cacheEntry, _jwtOptions.RefreshTokenLifeTime, ct);

            return token.Value;
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
        {
            var hash = HashToken(refreshToken);
            await _cache.RemoveByKeyAsync(_provider.RefreshToken(hash), ct);

            var record = await _refreshTokenRepository.GetTokenByHashAsync(hash, ct);

            if (record != null && !record.IsRevoked)
            {
                record.IsRevoked = true;
                record.RevokedAt = DateTime.UtcNow;
                await _refreshTokenRepository.UpdateTokenAsync(record, ct);
                _logger.LogDebug("Refresh token with hash {Hash} was revoked", hash);

                var remainingTTL = record.ExpiresAt - DateTime.UtcNow;
                if (remainingTTL > TimeSpan.Zero)
                {
                    var cacheEntry = new CacheEntry(record.UserId, record.ExpiresAt, true);
                    await _cache.SetAsync(_provider.RefreshToken(hash), cacheEntry, remainingTTL, ct);
                }
            }
            else _logger.LogDebug("Refresh token with hash {Hash} was not found in database or already revoked", hash);
        }

        public async Task<(string?, bool)> RotateRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
        {
            var oldHash = HashToken(refreshToken);
            var cacheKey = _provider.RefreshToken(oldHash);

            var existingCache = await _cache.GetAsync<CacheEntry>(cacheKey, ct);
            if (existingCache != null)
            {
                if (existingCache.IsRevoked || DateTime.UtcNow > existingCache.ExpiresAt)
                {
                    _logger.LogDebug("Fast fail: refresh token with hash {hash} is revoked or expired", oldHash);
                    return (null,  false);
                }
            }

            var oldToken = await _refreshTokenRepository.GetTokenByHashAsync(oldHash, ct);

            if (oldToken == null)
            {
                _logger.LogDebug("Provided refresh token with hash: {hash} was not found in database", oldHash);
                return (null, false);
            }
            if (oldToken.IsRevoked || oldToken.ExpiresAt < DateTime.UtcNow)
            {
                _logger.LogDebug("Provided refresh token is already revoked or expired at {ExpiredAt}", oldToken.ExpiresAt);

                var negativeCache = new CacheEntry(oldToken.UserId, oldToken.ExpiresAt, oldToken.IsRevoked);
                await _cache.SetAsync(cacheKey, negativeCache, _cacheOptions.DefaultTTL, ct);

                return (null, false);
            }

            var newToken = GenerateToken();

            var newTokenRecord = new RefreshToken
            {
                Id = Guid.NewGuid(),
                TokenHash = newToken.Hash,
                UserId = oldToken.UserId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.Add(_jwtOptions.RefreshTokenLifeTime),
                IsRevoked = false
            };

            oldToken.IsRevoked = true;
            oldToken.RevokedAt = DateTime.UtcNow;
            oldToken.ReplacedByTokenId = newTokenRecord.Id;

            try
            {
                await _refreshTokenRepository.AddTokenAsync(newTokenRecord, ct);
                await _refreshTokenRepository.UpdateTokenAsync(oldToken, ct);
                await _refreshTokenRepository.SaveChangesAsync(ct);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogDebug(ex, "Failed to update refresh token");
                return (null, false);
            }

            var oldRemainingTtl = oldToken.ExpiresAt - DateTime.UtcNow;
            if (oldRemainingTtl > TimeSpan.Zero)
            {
                var updatedCacheEntry = new CacheEntry(oldToken.UserId, oldToken.ExpiresAt, true);
                await _cache.SetAsync(cacheKey, updatedCacheEntry, oldRemainingTtl, ct);
            }

            var cacheEntry = new CacheEntry(newTokenRecord.UserId, newTokenRecord.ExpiresAt, false);
            await _cache.SetAsync(_provider.RefreshToken(newToken.Hash), cacheEntry, _jwtOptions.RefreshTokenLifeTime, ct);

            _logger.LogDebug("Token successfully rotated for user with id: {Id}", oldToken.Id);
            return (newToken.Value, true);
        }

        private static GeneratedToken GenerateToken()
        {
            var bytes = new byte[32];
            RandomNumberGenerator.Fill(bytes);
            var token = WebEncoders.Base64UrlEncode(bytes);
            var hash = Convert.ToHexString(SHA256.HashData(bytes));

            return new GeneratedToken(token, hash);
        }
        private static string HashToken(string token)
        {
            var bytes = WebEncoders.Base64UrlDecode(token);
            return Convert.ToHexString(SHA256.HashData(bytes));
        }

        readonly record struct GeneratedToken(string Value, string Hash);
        record CacheEntry(Guid UserId, DateTime ExpiresAt, bool IsRevoked);
    }
}
