using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.API.Infrastructure.Data.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private DatabaseClient _client;

        public AccountRepository(DatabaseClient client)
        {
            _client = client;
        }

        async public Task DeleteAsync(Guid id)
        {
            var account = await GetByIdAsync(id);
            if (account == null)
            {
                throw new KeyNotFoundException("Entity with given id does not exist");
            }
            _client.Accounts.Remove(account);
        }

        async public Task<IEnumerable<Account>> GetAllAsync()
        {
            return await _client.Accounts.ToListAsync();
        }

        async public Task<Account?> GetByIdAsync(Guid id)
        {
            return await _client.Accounts.FindAsync(id);
        }

        public void Save(Account account)
        {
            _client.Accounts.Add(account);
        }

        async public Task UpdateAsync(Account account)
        {
            var existingAccount = await GetByIdAsync(account.Id);
            if (existingAccount == null)
            {
                throw new KeyNotFoundException("Entity with given id does not exist");
            }
            _client.Accounts.Update(account);
        }

        public async Task SaveChangesAsync() => await _client.SaveChangesAsync();
    }
}
