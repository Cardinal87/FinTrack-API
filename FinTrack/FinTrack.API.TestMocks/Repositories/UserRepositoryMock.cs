using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Interfaces;
using FinTrack.API.Core.Exceptions;

namespace FinTrack.API.TestMocks.Repositories
{
    public class UserRepositoryMock : IUserRepository
    {
        private readonly List<User> _users = [];
        
        
        public void Add(User user)
        {
            _users.Add(user);
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

        public Task<IEnumerable<User>> GetAllAsync()
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
            var index = _users.IndexOf(user);
            if (index == -1)
            {
                throw new EntityNotFoundException("user with provided id not exists");
            }
            _users[index] = user;
            return Task.CompletedTask;
        }

        public void Reset()
        {
            _users.Clear();
        }
    }
}
