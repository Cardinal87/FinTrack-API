using FinTrack.API.Infrastructure.Data.Repositories;
using FinTrack.API.Core.Interfaces;
using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Common;
using FluentAssertions;
using FinTrack.IntegrationTests.Common;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using FinTrack.API.Infrastructure.Mappers;
using FinTrack.API.Infrastructure.Data.DbEntities;
using FinTrack.API.Core.Exceptions;
using FinTrack.API.TestMocks.Builders;

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
            var user = new UserBuilder().WithRoles(UserRoles.User, UserRoles.Admin)
                                        .Build();

            _userRepository.Add(user);
            await _userRepository.SaveChangesAsync();

            var savedUser = await _client.Users.FirstOrDefaultAsync(t => t.Id == user.Id);
            savedUser.Should().NotBeNull();
            savedUser.Email.Should().Be(user.Email);
            savedUser.Phone.Should().Be(user.Phone);
            savedUser.PasswordHash.Should().Be(user.PasswordHash);
            savedUser.Name.Should().Be(user.Name);
            savedUser.Roles.Should().Contain(UserRoles.Admin);
            savedUser.Roles.Should().Contain(UserRoles.User);

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
        async public Task GetUserById_RandomGuid_NullResult()
        {
            var result = await _userRepository.GetByIdAsync(Guid.NewGuid());

            result.Should().BeNull();
        }


        [Fact]
        async public Task UpdateUser_ValidData_Success()
        {

            var user = (await AddValidUsers(1))[0];

            user.Name = "updated_name";
            user.Email = "updated@email.com";
            user.Phone = "+78889997766";
            user.PasswordHash = "SHA256.50.Y0ea1poJCyWCd+yPum+ZQZov+ySJgVEGV8lEzNEUjpc=.XohImNooBHFR0OVvjcYpJ3NgPQ1qq73WKhHvch0VQtg=";
            user.AssignRole(UserRoles.Admin);

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            var updatedUser = await _client.Users.FirstOrDefaultAsync(t => t.Id == user.Id);
            updatedUser.Should().NotBeNull();
            updatedUser.Email.Should().Be(user.Email);
            updatedUser.Name.Should().Be(user.Name);
            updatedUser.Phone.Should().Be(user.Phone);
            updatedUser.PasswordHash.Should().Be(user.PasswordHash);
            updatedUser.Roles.Should().Contain(UserRoles.Admin);
        }


        [Fact]
        async public Task UpdateUser_UntrackedUser_ThrowEntityNotFoundException()
        {
            var user = new UserBuilder().Build();

            var update = async () => await _userRepository.UpdateAsync(user);

            await update.Should().ThrowAsync<EntityNotFoundException>();
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

        [Fact]
        async public Task DeleteUser_RandomGuid_ThrowEntityNotFoundException()
        {
            var delete = async () => await _userRepository.DeleteAsync(Guid.NewGuid());

            await delete.Should().ThrowAsync<EntityNotFoundException>();
        }

        [Fact]
        async public Task GetUserByEmail_ValidData_Success()
        {
            var user = (await AddValidUsers(1))[0];

            var savedUser = await _userRepository.GetByEmailAsync(user.Email);


            savedUser.Should().NotBeNull();
            savedUser.Email.Should().Be(user.Email);
            savedUser.Name.Should().Be(user.Name);
            savedUser.Phone.Should().Be(user.Phone);
            savedUser.PasswordHash.Should().Be(user.PasswordHash);
        }

        [Fact]
        async public Task GetUserByEmail_RandomEmail_NullResult()
        {
            var result = await _userRepository.GetByEmailAsync("random@gmail.com");

            result.Should().BeNull();
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
                var user = new UserBuilder().Build();

                var userDb = new UserDb()
                {
                    Id = user.Id,
                    Email = user.Email,
                    Phone = user.Phone,
                    Name = user.Name,
                    PasswordHash = user.PasswordHash
                };
                list.Add(user);
                _client.Users.Add(userDb);
            }
            await _client.SaveChangesAsync();

            return list;
        }

    }
}
