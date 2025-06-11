using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.API.Infrastructure.Data.Repositories
{
    class UserRepository : IUserRepository
    {
        private DatabaseClient _client;

        public UserRepository(DatabaseClient client)
        {
            _client = client;
        }

        async public Task DeleteAsync(Guid id)
        {
            var user = await GetByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException("Entity with given id does not exist");
            }
            _client.Users.Remove(user);
        }

        async public Task<IEnumerable<User>> GetAllAsync()
        {
            return await _client.Users.ToListAsync();
        }

        async public Task<User?> GetByIdAsync(Guid id)
        {
            return await _client.Users.FindAsync(id);
        }

        public void Add(User user)
        {
            _client.Users.Add(user);
        }

        async public Task UpdateAsync(User user)
        {
            var existingUser = await GetByIdAsync(user.Id);
            if (existingUser == null)
            {
                throw new KeyNotFoundException("Entity with given id does not exist");
            }
            _client.Users.Update(user);
        }

        async public Task SaveChangesAsync() => await _client.SaveChangesAsync();
    }
}
