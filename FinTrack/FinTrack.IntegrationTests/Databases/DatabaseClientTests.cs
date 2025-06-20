using Microsoft.EntityFrameworkCore;
using FinTrack.API.Core.Entities;
using FluentAssertions;
using FinTrack.IntegrationTests.Common;
using FinTrack.API.Infrastructure.Data.DbEntities;

namespace FinTrack.IntegrationTests.Databases
{
    public class DatabaseClientTests : DatabaseTestBase
    {
        
        [Fact]
        async public Task AddUser_WithExistingEmail_ThrownException()
        {
            var user = await CreateDefaultUser();

            var newName = "new_name";
            var newPhone = "+79998886677";

            var sameEmailUser = new UserDb()
            {
                Email = user.Email,
                Phone = newPhone,
                Name = newName,
                PasswordHash = user.PasswordHash
            };

            _client.Users.Add(sameEmailUser);
            var func = async () => await _client.SaveChangesAsync();
            await func.Should().ThrowAsync<DbUpdateException>();
        }

        [Fact]
        async public Task AddUser_WithExistingPhone_ThrownException()
        {
            var user = await CreateDefaultUser();

            var newName = "new_name";
            var newEmail = "new@email.com";

            var samePhoneUser = new UserDb()
            {
                Email = newEmail,
                Phone = user.Phone,
                Name = newName,
                PasswordHash = user.PasswordHash
            };

            _client.Users.Add(samePhoneUser);
            var func = async () => await _client.SaveChangesAsync();
            await func.Should().ThrowAsync<DbUpdateException>();
        }

        [Fact]
        async public Task AddUser_WithExistingName_ThrownException()
        {
            var user = await CreateDefaultUser();

            var newPhone = "+79998886677";
            var newEmail = "new@email.com";

            var sameNameUser = new UserDb()
            {
                Email = newEmail,
                Phone = newPhone,
                Name = user.Name,
                PasswordHash = user.PasswordHash
            };

            _client.Users.Add(sameNameUser);
            var func = async () => await _client.SaveChangesAsync();
            await func.Should().ThrowAsync<DbUpdateException>();
        }

        [Fact]
        async public Task DeleteUser_ValidData_DeleteAccount()
        {
            var user = await CreateDefaultUser();

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
        


        async private Task<UserDb> CreateDefaultUser()
        {
            var hash = "10a6e6cc8311a3e2bcc09bf6c199adecd5dd59408c343e926b129c4914f3cb01";
            var email = "test@email.com";
            var name = "test_user";
            var phone = "+79998887766";
            var user = new UserDb()
            {
                Email = email,
                Phone = phone,
                Name = name,
                PasswordHash = hash
            };

            _client.Users.Add(user);
            await _client.SaveChangesAsync();

            return user;
        }
    }
}
