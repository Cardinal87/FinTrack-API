using AutoMapper;
using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Interfaces;
using FinTrack.API.Infrastructure.Data.DbEntities;
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

        async public Task<IEnumerable<Transaction>> GetAllAsync()
        {
            var dbList = await _client.Transactions.ToListAsync();
            var transactionList = _mapper.Map<List<Transaction>>(dbList);
            return transactionList;
        }

        async public Task<IEnumerable<Transaction>> GetByDateAsync(DateOnly date)
        {
            var dbList = await _client.Transactions.Where(t => DateOnly.FromDateTime(t.Date) == date).ToListAsync();
            var transactionList = _mapper.Map<List<Transaction>>(dbList);
            return transactionList;
        }

        async public Task<IEnumerable<Transaction>> GetByDateAsync(DateOnly date, IEnumerable<Guid> accountIds)
        {
            var dbList = await _client.Transactions.Where(t => DateOnly.FromDateTime(t.Date) == date
                                                               && (accountIds.Contains(t.FromAccountId)
                                                               || accountIds.Contains(t.ToAccountId))).ToListAsync();
            var transactionList = _mapper.Map<List<Transaction>>(dbList);
            return transactionList;
        }


        async public Task<Transaction?> GetByIdAsync(Guid id)
        {
            var dbTransaction = await _client.Transactions.FindAsync(id);
            var transaction = _mapper.Map<Transaction>(dbTransaction);
            return transaction;
        }

        async public Task<IEnumerable<Transaction>> GetFromToDateAsync(DateTime fromDate, DateTime toDate)
        {
            var query = _client.Transactions.Where(t => t.Date >= fromDate && t.Date <= toDate);
            var dbList = await query.ToListAsync();
            var transactionList = _mapper.Map<List<Transaction>>(dbList);
            return transactionList;
        }

        async public Task<IEnumerable<Transaction>> GetFromToDateAsync(DateTime fromDate, DateTime toDate, IEnumerable<Guid> accountIds)
        {
            var query = _client.Transactions.Where(t => t.Date >= fromDate
                                                        && t.Date <= toDate 
                                                        && (accountIds.Contains(t.FromAccountId) || accountIds.Contains(t.ToAccountId)));
            var dbList = await query.ToListAsync();
            var transactionList = _mapper.Map<List<Transaction>>(dbList);
            return transactionList;
        }

        public void Add(Transaction transaction)
        {
            var dbTransaction = _mapper.Map<TransactionDb>(transaction);
            _client.Transactions.Add(dbTransaction);
        }

        async public Task SaveChangesAsync() => await _client.SaveChangesAsync();

       
    }
}
