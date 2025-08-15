
using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Exceptions;
using FinTrack.API.Core.Interfaces;

namespace FinTrack.API.TestMocks.Repositories
{
    public class AccountRepositoryMock : IAccountRepository
    {
        private readonly List<Account> _accounts = [];
        public void Add(Account account)
        {
            _accounts.Add(account);
        }

        public Task DeleteAsync(Guid id)
        {
            var acount = _accounts.FirstOrDefault(t => t.Id == id);
            if (acount == null)
            {
                throw new EntityNotFoundException("user with provided id not exists");
            }
            _accounts.Remove(acount);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Guid>> GetAccountIdsByUserIdAsync(Guid id)
        {
            return Task.FromResult(_accounts.Where(t => t.UserId == id).Select(t => t.Id));
        }

        public Task<IEnumerable<Account>> GetAllAsync()
        {
            return Task.FromResult(_accounts.AsEnumerable());
        }

        public Task<Account?> GetByIdAsync(Guid id)
        {
            return Task.FromResult(_accounts.FirstOrDefault(t => t.Id == id));
        }

        public Task SaveChangesAsync()
        {
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Account account)
        {
            var index = _accounts.IndexOf(account);
            if (index == -1)
            {
                throw new EntityNotFoundException("user with provided id not exists");
            }
            _accounts[index] = account;
            return Task.CompletedTask;
        }

        public void Reset()
        {
            _accounts.Clear();
        }
    }
}
