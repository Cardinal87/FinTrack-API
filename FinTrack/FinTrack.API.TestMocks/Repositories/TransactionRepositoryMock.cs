
using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Interfaces;

namespace FinTrack.API.TestMocks.Repositories
{
    public class TransactionRepositoryMock : ITransactionRepository
    {
        private readonly List<Transaction> _transactions = [];
        public void Add(Transaction transaction)
        {
            _transactions.Add(transaction);
        }

        public Task<IEnumerable<Transaction>> GetAllAsync()
        {
            return Task.FromResult(_transactions.AsEnumerable());
        }

        public Task<IEnumerable<Transaction>> GetByDateAsync(DateOnly date)
        {
            return Task.FromResult(_transactions.Where(t => DateOnly.FromDateTime(t.Date) == date));
        }

        public Task<IEnumerable<Transaction>> GetByDateAsync(DateOnly date, IEnumerable<Guid> accountIds)
        {
            return Task.FromResult(_transactions.Where(t => DateOnly.FromDateTime(t.Date) == date
            && (accountIds.Contains(t.ToAccountId) || accountIds.Contains(t.FromAccountId))));
        }

        public Task<Transaction?> GetByIdAsync(Guid id)
        {
            return Task.FromResult(_transactions.FirstOrDefault(t => t.Id == id));
        }

        public Task<IEnumerable<Transaction>> GetFromToDateAsync(DateTime fromDate, DateTime toDate)
        {
            return Task.FromResult(_transactions.Where(t => t.Date >= fromDate
                                                        && t.Date <= toDate));
        }

        public Task<IEnumerable<Transaction>> GetFromToDateAsync(DateTime fromDate, DateTime toDate, IEnumerable<Guid> accountIds)
        {
            return Task.FromResult(_transactions.Where(t => t.Date >= fromDate
                                                        && t.Date <= toDate
                                                        && (accountIds.Contains(t.FromAccountId) || accountIds.Contains(t.ToAccountId))));
        }

        public Task SaveChangesAsync()
        {
            return Task.CompletedTask;
        }

        public void Reset()
        {
            _transactions.Clear();
        }
    }
}
