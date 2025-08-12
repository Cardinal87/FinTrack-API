using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using FinTrack.IntegrationTests.Common;
using FinTrack.API.Infrastructure.Data.DbEntities;
using FinTrack.API.TestMocks.Builders;
using FinTrack.API.Core.Entities;

namespace FinTrack.IntegrationTests.Databases
{
    public class DatabaseClientTests : DatabaseTestBase
    {
        
        [Fact]
        async public Task AddUser_WithExistingEmail_ThrownException()
        {
            var user = new UserBuilder().BuildDbUser();
            
            _client.Users.Add(user);
            await _client.SaveChangesAsync();

            var sameEmailUser = new UserBuilder().WithEmail(user.Email).BuildDbUser();

            _client.Users.Add(sameEmailUser);
            var func = async () => await _client.SaveChangesAsync();
            await func.Should().ThrowAsync<DbUpdateException>();
        }

        [Fact]
        async public Task AddUser_WithExistingPhone_ThrownException()
        {
            var user = new UserBuilder().BuildDbUser();

            _client.Users.Add(user);
            await _client.SaveChangesAsync();

            var samePhoneUser = new UserBuilder().WithPhone(user.Phone).BuildDbUser();

            _client.Users.Add(samePhoneUser);
            var func = async () => await _client.SaveChangesAsync();
            await func.Should().ThrowAsync<DbUpdateException>();
        }

        [Fact]
        async public Task AddUser_WithExistingName_ThrownException()
        {
            var user = new UserBuilder().BuildDbUser();

            _client.Users.Add(user);
            await _client.SaveChangesAsync();

            var sameNameUser = new UserBuilder().WithName(user.Name).BuildDbUser();

            _client.Users.Add(sameNameUser);
            var func = async () => await _client.SaveChangesAsync();
            await func.Should().ThrowAsync<DbUpdateException>();
        }

        [Fact]
        async public Task DeleteUser_ValidData_DeleteAccount()
        {
            var user = new UserBuilder().BuildDbUser();
            _client.Users.Add(user);
            await _client.SaveChangesAsync();


            var firstAccount = new AccountDb()
            {
                UserId = user.Id,
                Balance = 0
            };
            var secondAccount = new AccountDb()
            {
                UserId = user.Id,
                Balance = 0
            };

            _client.Accounts.Add(firstAccount);
            _client.Accounts.Add(secondAccount);
            await _client.SaveChangesAsync();

            _client.Users.Remove(user);
            await _client.SaveChangesAsync();

            var accounts = await _client.Accounts.Where(t => t.UserId == user.Id).ToListAsync();
            accounts.Should().HaveCount(0);

        }
        
    }
}
