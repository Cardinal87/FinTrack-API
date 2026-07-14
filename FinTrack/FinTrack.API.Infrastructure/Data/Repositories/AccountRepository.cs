using AutoMapper;
using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Exceptions;
using FinTrack.API.Core.Interfaces;
using FinTrack.API.Infrastructure.Common.DTO;
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
                throw new EntityNotFoundException($"entity with id {id} does not exist");
            }
            _client.Accounts.Remove(account);
        }

        async public Task<IEnumerable<Account>> GetAllAsync(int pageNumber = 1, int pageSize = 50)
        {
            if (pageSize > 1000)
            {
                throw new ArgumentException($"Page size is too large - {pageSize}");
            }
            var skip = (pageNumber - 1) * pageSize;
            var pagedData = await _client.Accounts
                .AsNoTracking()
                .OrderBy(t => t.Id)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
            var accountList = _mapper.Map<List<Account>>(pagedData);
            return accountList;
        }

        async public Task<Account?> GetByIdAsync(Guid id)
        {
            var dbAccount = await _client.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);
            var account = _mapper.Map<Account>(dbAccount);
            return account;
        }

        public Task AddAsync(Account account)
        {
            var dbAccount = _mapper.Map<AccountDTO>(account);
            _client.Accounts.Add(dbAccount);
            return Task.CompletedTask;
        }

        async public Task UpdateAsync(Account account)
        {
            var existingAccount = await _client.Accounts.FindAsync(account.Id);
            if (existingAccount == null)
            {
                throw new EntityNotFoundException($"entity with id {account.Id} does not exist");
            }
            _mapper.Map(account, existingAccount);
        }

        public async Task<IEnumerable<Guid>> GetAccountIdsByUserIdAsync(Guid id)
        {
            var accountIds = await _client.Accounts
                .Where(x => x.UserId == id)
                .AsNoTracking()
                .Select(x => x.Id)
                .ToListAsync();

            return accountIds;
        }


        public async Task SaveChangesAsync() => await _client.SaveChangesAsync();

        
    }
}
