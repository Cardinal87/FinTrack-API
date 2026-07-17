
using AutoMapper;
using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Interfaces;
using FinTrack.API.Infrastructure.Caching.DTO;
using FinTrack.API.Infrastructure.Common.DTO;
using FinTrack.API.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FinTrack.API.Infrastructure.Caching.Decorators
{
    public class CachedUserRepositoryDecorator : IUserRepository
    {
        private readonly IUserRepository _inner;
        private readonly ILogger<CachedUserRepositoryDecorator> _logger;
        private readonly ICacheKeyProvider _provider;
        private readonly ICacheService _cache;
        private readonly CacheOptions _cacheOptions;
        private readonly IMapper _mapper;

        public CachedUserRepositoryDecorator(IUserRepository inner, 
                                             ILogger<CachedUserRepositoryDecorator> logger, 
                                             ICacheKeyProvider provider,
                                             ICacheService cache,
                                             IMapper mapper,
                                             IOptions<CacheOptions> cacheOptions)
        {
            _inner = inner;
            _logger = logger;
            _provider = provider;
            _cache = cache;
            _mapper = mapper;
            _cacheOptions = cacheOptions.Value;
        }

        async public Task AddAsync(User user)
        {
            await _inner.AddAsync(user);

            var idKey = _provider.UserById(user.Id);
            var emailKey = _provider.UserByEmail(user.Email);

            var mapped = _mapper.Map<UserDb>(user);

            await _cache.RemoveByPatternAsync(_provider.UserListPattern());
            await _cache.SetAsync(idKey, mapped, _cacheOptions.DefaultTTL);
            await _cache.SetAsync(emailKey, mapped, _cacheOptions.DefaultTTL);
        }

        async public Task DeleteAsync(Guid id)
        {
            var user = await _inner.GetByIdAsync(id);
            await _inner.DeleteAsync(id);

            await _cache.RemoveByKeyAsync(_provider.UserById(id));
            if (user != null) await _cache.RemoveByKeyAsync(_provider.UserByEmail(user.Email));
            await _cache.RemoveByPatternAsync(_provider.UserListPattern());
        }

        async public Task UpdateAsync(User user)
        {
            var oldUser = await _inner.GetByIdAsync(user.Id);
            await _inner.UpdateAsync(user);

            
            await _cache.RemoveByKeyAsync(_provider.UserById(user.Id));
            if (oldUser != null) await _cache.RemoveByKeyAsync(_provider.UserByEmail(oldUser.Email));
            
            await _cache.RemoveByPatternAsync(_provider.UserListPattern());
        }

        async public Task<IEnumerable<User>> GetAllAsync(int pageNumber = 1, int pageSize = 50)
        {
            var key = _provider.UserAll(pageNumber, pageSize);
            var cachedList = await _cache.GetAsync<List<UserDb>>(key);
            if (cachedList != null)
            {
                _logger.LogDebug("Cache found successfully");
                return _mapper.Map<List<User>>(cachedList);
            }
            _logger.LogDebug("Cache miss. Falling into database");

            var dbList = await _inner.GetAllAsync(pageNumber, pageSize);

            
            var mappedList = _mapper.Map<List<UserDb>>(dbList);
            _logger.LogDebug("Trying restore value to cache");
            await _cache.SetAsync(key, mappedList, _cacheOptions.DefaultTTL);
            

            return dbList;
        }

        async public Task<User?> GetByEmailAsync(string email)
        {
            var key = _provider.UserByEmail(email);
            var cachedUser = await _cache.GetAsync<UserDb>(key);
            if (cachedUser != null)
            {
                _logger.LogDebug("Cache found successfully");
                return _mapper.Map<User>(cachedUser);
            }
            _logger.LogDebug("Cache miss. Falling into database");
            var dbUser = await _inner.GetByEmailAsync(email);

            if (dbUser != null)
            {
                var mappedUser = _mapper.Map<UserDb>(dbUser);
                _logger.LogDebug("Trying restore value to cache");
                await _cache.SetAsync(key, mappedUser, _cacheOptions.DefaultTTL);
            }

            return dbUser;
        }

        async public Task<User?> GetByIdAsync(Guid id)
        {
            var key = _provider.UserById(id);
            var cachedUser = await _cache.GetAsync<UserDb>(key);
            if (cachedUser != null)
            {
                _logger.LogDebug("Cache found successfully");
                return _mapper.Map<User>(cachedUser);
            }
            _logger.LogDebug("Cache miss. Falling into database");
            var dbUser = await _inner.GetByIdAsync(id);

            if (dbUser != null)
            {
                var mappedUser = _mapper.Map<UserDb>(dbUser);
                _logger.LogDebug("Trying restore value to cache");
                await _cache.SetAsync(key, mappedUser, _cacheOptions.DefaultTTL);
            }

            return dbUser;
        }

        public Task SaveChangesAsync()
        {
            return _inner.SaveChangesAsync();
        }

        
    }
}
