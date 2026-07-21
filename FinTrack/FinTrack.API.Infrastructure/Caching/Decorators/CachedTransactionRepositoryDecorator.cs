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
    public class CachedTransactionRepositoryDecorator : ITransactionRepository
    {
        private readonly ITransactionRepository _inner;
        private readonly ILogger<CachedTransactionRepositoryDecorator> _logger;
        private readonly ICacheKeyProvider _provider;
        private readonly ICacheService _cache;
        private readonly CacheOptions _cacheOptions;
        private readonly IMapper _mapper;

        public CachedTransactionRepositoryDecorator(ITransactionRepository inner,
                                             ILogger<CachedTransactionRepositoryDecorator> logger,
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
        public async Task<Transaction?> GetByIdAsync(Guid id)
        {
            var key = _provider.TransactionsById(id);
            var cachedTransaction = await _cache.GetAsync<TransactionDb>(key);

            if (cachedTransaction != null) {
                _logger.LogDebug("Cache found successfully");
                return _mapper.Map<Transaction>(cachedTransaction);
            }

            _logger.LogDebug("Cache miss. Falling into database");
            var dbTransaction = await _inner.GetByIdAsync(id);

            if (dbTransaction != null)
            {
                var mapped = _mapper.Map<TransactionDb>(dbTransaction);
                _logger.LogDebug("Trying restore value to cache");
                await _cache.SetAsync(key, mapped, _cacheOptions.DefaultTTL);
            }
            return dbTransaction;
        }

        public Task AddAsync(Transaction transaction)
        {
            return _inner.AddAsync(transaction);
        }

        public Task<IEnumerable<Transaction>> GetAccountTransactionsAsync(Guid accountId, int pageNumber = 1, int pageSize = 50)
        {
            return _inner.GetAccountTransactionsAsync(accountId, pageNumber, pageSize);
        }

        public Task<IEnumerable<Transaction>> GetAllAsync(int pageNumber = 1, int pageSize = 50)
        {
            return _inner.GetAllAsync(pageNumber, pageSize);
        }

        public Task<IEnumerable<Transaction>> GetByDateAsync(DateOnly date, int pageNumber = 1, int pageSize = 50)
        {
            return _inner.GetByDateAsync(date, pageNumber, pageSize);
        }

        public Task<IEnumerable<Transaction>> GetByDateAsync(DateOnly date, IEnumerable<Guid> accountIds, int pageNumber = 1, int pageSize = 50)
        {
            return _inner.GetByDateAsync(date, accountIds, pageNumber, pageSize);
        }

        public Task<IEnumerable<Transaction>> GetFromToDateAsync(DateTime fromDate, DateTime toDate, int pageNumber = 1, int pageSize = 50)
        {
            return _inner.GetFromToDateAsync(fromDate, toDate, pageNumber, pageSize);
        }

        public Task<IEnumerable<Transaction>> GetFromToDateAsync(DateTime fromDate, DateTime toDate, IEnumerable<Guid> accountIds, int pageNumber = 1, int pageSize = 50)
        {
            return _inner.GetFromToDateAsync(fromDate, toDate, accountIds, pageNumber, pageSize);
        }

    }
}
