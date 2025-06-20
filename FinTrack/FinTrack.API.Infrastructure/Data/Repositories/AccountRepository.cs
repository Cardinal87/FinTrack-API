using AutoMapper;
using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Interfaces;
using FinTrack.API.Infrastructure.Data.DbEntities;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.API.Infrastructure.Data.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private DatabaseClient _client;
        private IMapper _mapper;

        public AccountRepository(DatabaseClient client, IMapper mapper)
        {
            _client = client;
            _mapper = mapper;
        }

        async public Task DeleteAsync(Guid id)
        {
            var account = await _client.Accounts.FindAsync(id);
            if (account == null)
            {
                throw new KeyNotFoundException("Entity with given id does not exist");
            }
            _client.Accounts.Remove(account);
        }

        async public Task<IEnumerable<Account>> GetAllAsync()
        {
            var dbList = await _client.Accounts.ToListAsync();
            var accountList = _mapper.Map<List<Account>>(dbList);
            return accountList;
        }

        async public Task<Account?> GetByIdAsync(Guid id)
        {
            var dbAccount = await _client.Accounts.FindAsync(id);
            var account = _mapper.Map<Account>(dbAccount);
            return account;
        }

        public void Add(Account account)
        {
            var dbAccount = _mapper.Map<AccountDb>(account);
            _client.Accounts.Add(dbAccount);
        }

        async public Task UpdateAsync(Account account)
        {
            var existingAccount = await _client.Accounts.FindAsync(account.Id);
            if (existingAccount == null)
            {
                throw new KeyNotFoundException("Entity with given id does not exist");
            }
            _mapper.Map(account, existingAccount);
        }

        public async Task SaveChangesAsync() => await _client.SaveChangesAsync();
    }
}
