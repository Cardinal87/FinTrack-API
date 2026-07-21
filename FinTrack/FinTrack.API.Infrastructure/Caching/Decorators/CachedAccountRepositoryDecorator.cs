using AutoMapper;
using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Interfaces;
using FinTrack.API.Infrastructure.Caching.DTO;
using FinTrack.API.Infrastructure.Common.DTO;
using FinTrack.API.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Principal;

namespace FinTrack.API.Infrastructure.Caching.Decorators
{
    public class CachedAccountRepositoryDecorator : IAccountRepository
    {

        private readonly IAccountRepository _inner;
        private readonly ILogger<CachedAccountRepositoryDecorator> _logger;
        private readonly ICacheKeyProvider _provider;
        private readonly ICacheService _cache;
        private readonly CacheOptions _cacheOptions;
        private readonly IMapper _mapper;

        public CachedAccountRepositoryDecorator(IAccountRepository inner,
                                             ILogger<CachedAccountRepositoryDecorator> logger,
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
        public async Task AddAsync(Account account)
        {
            await _inner.AddAsync(account);

            var idKey = _provider.AccountById(account.Id);

            await _cache.RemoveByKeyAsync(_provider.AccountsByUserId(account.UserId));
            await _cache.RemoveByPatternAsync(_provider.AccountListPattern());

            await _cache.SetAsync(idKey, account, _cacheOptions.DefaultTTL);
        }

        public async Task DeleteAsync(Guid id)
        {
            var account = await _inner.GetByIdAsync(id);
            await _inner.DeleteAsync(id);

            await _cache.RemoveByKeyAsync(_provider.AccountById(id));
            if (account != null) await _cache.RemoveByKeyAsync(_provider.AccountsByUserId(account.UserId));
            await _cache.RemoveByPatternAsync(_provider.AccountListPattern());
        }

        public async Task UpdateAsync(Account account)
        {
            await _inner.UpdateAsync(account);

            await _cache.RemoveByKeyAsync(_provider.AccountById(account.Id));
            if (account != null) await _cache.RemoveByKeyAsync(_provider.AccountsByUserId(account.UserId));
            await _cache.RemoveByPatternAsync(_provider.AccountListPattern());
        }

        public async Task<IEnumerable<Guid>> GetAccountIdsByUserIdAsync(Guid id)
        {
            var key = _provider.AccountsByUserId(id);
            var cachedList = await _cache.GetAsync<List<Guid>>(key);

            if (cachedList != null)
            {
                _logger.LogDebug("Cache found successfully");
                return cachedList;
            }

            _logger.LogDebug("Cache miss. Falling into database");
            var dbList = await _inner.GetAccountIdsByUserIdAsync(id);

            if (dbList.Any())
            {
                _logger.LogDebug("Trying restore value to cache");
                await _cache.SetAsync(key, dbList, _cacheOptions.DefaultTTL);
            }

            return dbList;
        }

        public async Task<IEnumerable<Account>> GetAllAsync(int pageNumber = 1, int pageSize = 50)
        {
            var key = _provider.AccountAll(pageNumber, pageSize);
            var cachedList = await _cache.GetAsync<List<AccountDb>>(key);

            if (cachedList != null)
            {
                _logger.LogDebug("Cache found successfully");
                return _mapper.Map<List<Account>>(cachedList);
            }
            
            _logger.LogDebug("Cache miss. Falling into database");
            var dbList = await _inner.GetAllAsync(pageNumber, pageSize);

            _logger.LogDebug("Trying restore value to cache");
            var mapped = _mapper.Map<List<AccountDb>>(dbList);
            await _cache.SetAsync(key, mapped, _cacheOptions.DefaultTTL);
            

            return dbList;
        }

        public async Task<Account?> GetByIdAsync(Guid id)
        {
            var key = _provider.AccountById(id);
            var cachedAccount = await _cache.GetAsync<AccountDb>(key);

            if (cachedAccount != null)
            {
                _logger.LogDebug("Cache found successfully");
                return _mapper.Map<Account>(cachedAccount);
            }

            _logger.LogDebug("Cache miss. Falling into database");
            var dbAccount = await _inner.GetByIdAsync(id);

            if (dbAccount != null)
            {
                _logger.LogDebug("Trying restore value to cache");
                var mapped = _mapper.Map<AccountDb>(dbAccount);
                await _cache.SetAsync(key, mapped, _cacheOptions.DefaultTTL);
            }
            return dbAccount;

        }

    }
}
