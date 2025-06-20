using FinTrack.API.Infrastructure.Data.Repositories;
using FinTrack.API.Core.Interfaces;
using FinTrack.API.Core.Entities;
using FluentAssertions;
using FinTrack.IntegrationTests.Common;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using FinTrack.API.Infrastructure.Mappers;
using FinTrack.API.Infrastructure.Data.DbEntities;

namespace FinTrack.IntegrationTests.Repositories
{
    public class UserRepositoryTests : DatabaseTestBase
    {
        private IUserRepository _userRepository = null!;
        
        override async public Task InitializeAsync()
        {
            await base.InitializeAsync();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<UserMapper>();
                
            });

            IMapper mapper = config.CreateMapper();
            _userRepository = new UserRepository(_client, mapper);
        }


        [Fact]
        async public Task AddUser_ValidData_Success()
        {
            var hash = "10a6e6cc8311a3e2bcc09bf6c199adecd5dd59408c343e926b129c4914f3cb01";
            var email = "test@email.com";
            var name = "test_user";
            var phone = "+79998887766";
            var user = new User(email, phone, name, hash);

            _userRepository.Add(user);
            await _userRepository.SaveChangesAsync();

            var savedUser = await _client.Users.FirstOrDefaultAsync(t => t.Id == user.Id);
            savedUser.Should().NotBeNull();

        }

        [Fact]
        async public Task GetUserById_ValidData_Success()
        {
            var user = (await AddValidUsers(1))[0];

            var savedUser = await _userRepository.GetByIdAsync(user.Id);

            savedUser.Should().NotBeNull();
            savedUser.Email.Should().Be(user.Email);
            savedUser.Name.Should().Be(user.Name);
            savedUser.Phone.Should().Be(user.Phone);
            savedUser.PasswordHash.Should().Be(user.PasswordHash);
        }

        [Fact]
        async public Task UpdateUser_ValidData_Success()
        {

            var user = (await AddValidUsers(1))[0];

            user.Name = "updated_name";
            user.Email = "updated@email.com";
            user.Phone = "+78889997766";
            user.PasswordHash = "10a6e6cc8311a3e2bcc09bf6c199adecd5dd59408c343e926b129c4914f3cb02";

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            var updatedUser = await _client.Users.FirstOrDefaultAsync(t => t.Id == user.Id);
            updatedUser.Should().NotBeNull();
            updatedUser.Email.Should().Be(user.Email);
            updatedUser.Name.Should().Be(user.Name);
            updatedUser.Phone.Should().Be(user.Phone);
            updatedUser.PasswordHash.Should().Be(user.PasswordHash);
        }

        [Fact]
        async public Task GetAllUsers_ValidData_Success()
        {
            var userList = await AddValidUsers(3);
            
            var user1 = userList[0];
            var user2 = userList[1];
            var user3 = userList[2];

            var users = await _userRepository.GetAllAsync();
            var recievedList = users.ToList();

            recievedList.Should().HaveCount(3);
            var userDb1 = recievedList.FirstOrDefault(t => t.Id == user1.Id);
            var userDb2 = recievedList.FirstOrDefault(t => t.Id == user2.Id);
            var userDb3 = recievedList.FirstOrDefault(t => t.Id == user3.Id);


            userDb1.Should().NotBeNull();
            userDb2.Should().NotBeNull();
            userDb3.Should().NotBeNull();
        }


        [Fact]
        async public Task DeleteUser_ValidData_Success()
        {
            var user = (await AddValidUsers(1))[0];

            await _userRepository.DeleteAsync(user.Id);
            await _userRepository.SaveChangesAsync();

            var deletedUser = _client.Users.FirstOrDefault(t => t.Id == user.Id);
            deletedUser.Should().BeNull();
        }
        
        /// <summary>
        /// Method for add a valid users to databese
        /// </summary>
        /// <param name="amount">
        /// Amount of users that must be added.
        /// Must be less or equal than 10 and greater than 0
        /// </param>
        /// <returns>
        /// <see cref="List{T}"/> with add added uses
        /// </returns>
        async private Task<List<User>> AddValidUsers(int amount)
        {
            var list = new List<User>();
            for (int nonce = 0; nonce < amount; nonce++)
            {
                var hash = $"10a6e6cc8311a3e2bcc09bf6c199adecd5dd59408c343e926b129c4914f3cb0{nonce}";
                var email = $"test{nonce}@email.com";
                var name = $"test_user{nonce}";
                var phone = $"+7999888776{nonce}";

                var user = new User(email, phone, name, hash);

                var userDb = new UserDb()
                {
                    Id = user.Id,
                    Email = email,
                    Phone = phone,
                    Name = name,
                    PasswordHash = hash
                };
                list.Add(user);
                _client.Users.Add(userDb);
            }
            await _client.SaveChangesAsync();

            return list;
        }

    }
}
