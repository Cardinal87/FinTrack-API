using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.API.Infrastructure.Data.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private DatabaseClient _client;

        public TransactionRepository(DatabaseClient client)
        {
            _client = client;
        }

        async public Task<IEnumerable<Transaction>> GetAllAsync()
        {
            return await _client.Transactions.ToListAsync();
        }

        async public Task<IEnumerable<Transaction>> GetByDateAsync(DateTime date)
        {
            return await _client.Transactions.Where(t => DateTime.Compare(t.Date.Date, date.Date) == 0).ToListAsync();
        }

        async public Task<Transaction?> GetByIdAsync(Guid id)
        {
            return await _client.Transactions.FindAsync(id);
        }

        async public Task<IEnumerable<Transaction>> GetFromToDateAsync(DateTime fromDate, DateTime toDate)
        {
            var fromDateComp = fromDate.Date;
            var toDateComp = toDate.Date;
            var query = _client.Transactions.Where(t => DateTime.Compare(t.Date.Date, fromDate) >= 0 && DateTime.Compare(t.Date.Date, toDate) <= 0);
            return await query.ToListAsync();
        }

        public void Add(Transaction transaction)
        {
            _client.Transactions.Add(transaction);
        }

        async public Task SaveChangesAsync() => await _client.SaveChangesAsync();
    }
}
