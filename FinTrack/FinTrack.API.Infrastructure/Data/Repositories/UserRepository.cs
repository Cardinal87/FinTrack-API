using AutoMapper;
using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Exceptions;
using FinTrack.API.Core.Interfaces;
using FinTrack.API.Infrastructure.Common.DTO;
using Microsoft.EntityFrameworkCore;
using Npgsql;

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

        async public Task<IEnumerable<User>> GetAllAsync(int pageNumber = 1, int pageSize = 50)
        {
            if (pageSize > 1000)
            {
                throw new ArgumentException($"Page size is too large - {pageSize}");
            }
            var skip = (pageNumber - 1) * pageSize;
            var pagedData = await _client.Users
                .OrderBy(t => t.Id)
                .AsNoTracking()
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
            var userList = _mapper.Map<List<User>>(pagedData);
            return userList.AsReadOnly();
        }

        async public Task<User?> GetByIdAsync(Guid id)
        {
            var dbUser = await _client.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);
            var user = _mapper.Map<User>(dbUser);
            return user;
        }
        public async Task<User?> GetByEmailAsync(string email)
        {
            var dbUser = await _client.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Email == email);
            var user = _mapper.Map<User>(dbUser);
            return user;
        }

        public Task AddAsync(User user)
        {
            var dbUser = _mapper.Map<UserDTO>(user);
            _client.Users.Add(dbUser);
            return Task.CompletedTask;
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

        async public Task SaveChangesAsync()
        {
            try
            {
                await _client.SaveChangesAsync();
            }
            catch(DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
            {
                var property = pgEx.ConstraintName ?? "Unknown";
                throw new UniqueConstraintViolationException(property);
            }
        }

        
    }
}
