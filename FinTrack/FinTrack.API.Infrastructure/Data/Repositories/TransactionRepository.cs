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

        async public Task<IEnumerable<Transaction>> GetByDateAsync(DateTime date)
        {
            var dbList = await _client.Transactions.Where(t => DateTime.Compare(t.Date.Date, date.Date) == 0).ToListAsync();
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
            var fromDateComp = fromDate.Date;
            var toDateComp = toDate.Date;
            var query = _client.Transactions.Where(t => DateTime.Compare(t.Date.Date, fromDate) >= 0 && DateTime.Compare(t.Date.Date, toDate) <= 0);
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
