using AutoMapper;
using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Interfaces;
using FinTrack.API.Infrastructure.Common.DTO;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.API.Infrastructure.Data.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private DatabaseClient _client;
        private IMapper _mapper;

        public TransactionRepository(DatabaseClient client, IMapper mapper)
        {
            _client = client;
            _mapper = mapper;
        }

        async public Task<IEnumerable<Transaction>> GetAllAsync(int pageNumber = 1, int pageSize = 50)
        {
            if (pageSize > 1000)
            {
                throw new ArgumentException($"Page size is too large - {pageSize}");
            }
            var skip = (pageNumber - 1) * pageSize;
            var pagedData = await _client.Transactions
                .OrderBy(t => t.Id)
                .AsNoTracking()
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
            var transactionList = _mapper.Map<List<Transaction>>(pagedData);
            return transactionList;
        }

        async public Task<IEnumerable<Transaction>> GetByDateAsync(DateOnly date, int pageNumber = 1, int pageSize = 50)
        {
            if (pageSize > 1000)
            {
                throw new ArgumentException($"Page size is too large - {pageSize}");
            }
            var skip = (pageNumber - 1) * pageSize;
            var pagedData = await _client.Transactions
                .Where(t => DateOnly.FromDateTime(t.Date) == date)
                .OrderBy(t => t.Id)
                .AsNoTracking()
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
            var transactionList = _mapper.Map<List<Transaction>>(pagedData);
            return transactionList;
        }

        async public Task<IEnumerable<Transaction>> GetAccountTransactionsAsync(Guid accountId, int pageNumber = 1, int pageSize = 50)
        {
            if (pageSize > 1000)
            {
                throw new ArgumentException($"Page size is too large - {pageSize}");
            }
            var skip = (pageNumber - 1) * pageSize;
            var pagedData = await _client.Transactions
                .Where(t => t.FromAccountId == accountId || t.ToAccountId == accountId)
                .OrderBy(t => t.Id)
                .AsNoTracking()
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
            var transactionList = _mapper.Map<List<Transaction>>(pagedData);
            return transactionList;
        }


        async public Task<IEnumerable<Transaction>> GetByDateAsync(DateOnly date, IEnumerable<Guid> accountIds, int pageNumber = 1, int pageSize = 50)
        {
            if (pageSize > 1000)
            {
                throw new ArgumentException($"Page size is too large - {pageSize}");
            }
            var skip = (pageNumber - 1) * pageSize;

            var pagedData = await _client.Transactions
                .Where(t => DateOnly.FromDateTime(t.Date) == date
                                                && (accountIds.Contains(t.FromAccountId)
                                                || accountIds.Contains(t.ToAccountId)))
                .OrderBy(t => t.Id)
                .AsNoTracking()
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
            var transactionList = _mapper.Map<List<Transaction>>(pagedData);
            return transactionList;
        }


        async public Task<Transaction?> GetByIdAsync(Guid id)
        {
            var dbTransaction = await _client.Transactions
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);
            var transaction = _mapper.Map<Transaction>(dbTransaction);
            return transaction;
        }

        async public Task<IEnumerable<Transaction>> GetFromToDateAsync(DateTime fromDate, DateTime toDate, int pageNumber = 1, int pageSize = 50)
        {
            if (pageSize > 1000)
            {
                throw new ArgumentException($"Page size is too large - {pageSize}");
            }
            var skip = (pageNumber - 1) * pageSize;

            var pagedData = await _client.Transactions
                .Where(t => t.Date >= fromDate && t.Date <= toDate)
                .OrderBy(t => t.Id)
                .AsNoTracking()
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
            var transactionList = _mapper.Map<List<Transaction>>(pagedData);
            return transactionList;
        }

        async public Task<IEnumerable<Transaction>> GetFromToDateAsync(DateTime fromDate, DateTime toDate, IEnumerable<Guid> accountIds, int pageNumber = 1, int pageSize = 50)
        {
            if (pageSize > 1000)
            {
                throw new ArgumentException($"Page size is too large - {pageSize}");
            }
            var skip = (pageNumber - 1) * pageSize;

            var pagedData = await _client.Transactions
                .Where(t => t.Date >= fromDate
                       && t.Date <= toDate
                       && (accountIds.Contains(t.FromAccountId) || accountIds.Contains(t.ToAccountId)))
                .OrderBy(t => t.Id)
                .AsNoTracking()
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
            var transactionList = _mapper.Map<List<Transaction>>(pagedData);
            return transactionList;
        }

        public Task AddAsync(Transaction transaction)
        {
            var dbTransaction = _mapper.Map<TransactionDTO>(transaction);
            _client.Transactions.Add(dbTransaction);
            return Task.CompletedTask;
        }

        async public Task SaveChangesAsync() => await _client.SaveChangesAsync();

        
    }
}
