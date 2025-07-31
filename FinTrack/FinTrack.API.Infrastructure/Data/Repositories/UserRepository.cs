using AutoMapper;
using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Exceptions;
using FinTrack.API.Core.Interfaces;
using FinTrack.API.Infrastructure.Data.DbEntities;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.API.Infrastructure.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private DatabaseClient _client;
        private IMapper _mapper;
        public UserRepository(DatabaseClient client, IMapper mapper)
        {
            _client = client;
            _mapper = mapper;
        }

        async public Task DeleteAsync(Guid id)
        {
            var user = await _client.Users.FindAsync(id);
            if (user == null)
            {
                throw new EntityNotFoundException($"entity with id {id} does not exist");
            }
            _client.Users.Remove(user);
        }

        async public Task<IEnumerable<User>> GetAllAsync()
        {
            var dbList = await _client.Users.ToListAsync();
            var userList = _mapper.Map<List<User>>(dbList);
            return userList;
        }

        async public Task<User?> GetByIdAsync(Guid id)
        {
            var dbUser = await _client.Users.FindAsync(id);
            var user = _mapper.Map<User>(dbUser);
            return user;
        }
        public async Task<User?> GetByEmailAsync(string email)
        {
            var dbUser = await _client.Users.FirstOrDefaultAsync(t => t.Email == email);
            var user = _mapper.Map<User>(dbUser);
            return user;
        }

        public void Add(User user)
        {
            var dbUser = _mapper.Map<UserDb>(user);
            _client.Users.Add(dbUser);
        }

        async public Task UpdateAsync(User user)
        {
            var existingUser = await _client.Users.FindAsync(user.Id);
            if (existingUser == null)
            {
                throw new EntityNotFoundException($"entity with id {user.Id} does not exist");
            }
            _mapper.Map(user, existingUser);
        }

        async public Task SaveChangesAsync() => await _client.SaveChangesAsync();

        
    }
}
