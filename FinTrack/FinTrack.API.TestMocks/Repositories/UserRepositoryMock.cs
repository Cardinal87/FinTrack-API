using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Interfaces;
using FinTrack.API.Core.Exceptions;

namespace FinTrack.API.TestMocks.Repositories
{
    public class UserRepositoryMock : IUserRepository
    {
        private readonly List<User> _users = [];
        
        
        public Task AddAsync(User user)
        {
            _users.Add(user);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id)
        {
            var user = _users.FirstOrDefault(t => t.Id == id);
            if (user == null)
            {
                throw new EntityNotFoundException("user with provided id not exists");
            }
            _users.Remove(user);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<User>> GetAllAsync(int pageNumber = 1, int pageSize = 50)
        {
            return Task.FromResult(_users.AsEnumerable());
        }

        public Task<User?> GetByEmailAsync(string email)
        {
            return Task.FromResult(_users.FirstOrDefault(t => t.Email == email));
        }

        public Task<User?> GetByIdAsync(Guid id)
        {
            return Task.FromResult(_users.FirstOrDefault(t => t.Id == id));
        }

        public Task SaveChangesAsync()
        {
            return Task.CompletedTask;
        }

        public Task UpdateAsync(User user)
        {
            return Task.CompletedTask;
        }

        public void Reset()
        {
            _users.Clear();
        }
    }
}
