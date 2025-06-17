using Microsoft.EntityFrameworkCore;
using FinTrack.API.Core.Entities;
using FluentAssertions;
using FinTrack.IntegrationTests.Common;

namespace FinTrack.IntegrationTests.Databases
{
    public class DatabaseClientTests : DatabaseTestBase
    {
        
        [Fact]
        async public Task AddUser_WithExistingEmail_ThrownException()
        {
            var hash = "10a6e6cc8311a3e2bcc09bf6c199adecd5dd59408c343e926b129c4914f3cb01";
            var email = "test@email.com";
            var name = "test_user";
            var phone = "+79998887766";
            var user = new User(email, phone, name, hash);

            _client.Users.Add(user);
            await _client.SaveChangesAsync();

            var newName = "new_name";
            var newPhone = "+79998886677";

            var sameEmailUser = new User(email, newPhone, newName, hash);

            _client.Users.Add(sameEmailUser);
            var func = async () => await _client.SaveChangesAsync();
            await func.Should().ThrowAsync<DbUpdateException>();
        }

        [Fact]
        async public Task AddUser_WithExistingPhone_ThrownException()
        {
            var hash = "10a6e6cc8311a3e2bcc09bf6c199adecd5dd59408c343e926b129c4914f3cb01";
            var email = "test@email.com";
            var name = "test_user";
            var phone = "+79998887766";
            var user = new User(email, phone, name, hash);

            _client.Users.Add(user);
            await _client.SaveChangesAsync();

            var newName = "new_name";
            var newEmail = "new@email.com";

            var samePhoneUser = new User(newEmail, phone, newName, hash);

            _client.Users.Add(samePhoneUser);
            var func = async () => await _client.SaveChangesAsync();
            await func.Should().ThrowAsync<DbUpdateException>();
        }

        [Fact]
        async public Task AddUser_WithExistingName_ThrownException()
        {
            var hash = "10a6e6cc8311a3e2bcc09bf6c199adecd5dd59408c343e926b129c4914f3cb01";
            var email = "test@email.com";
            var name = "test_user";
            var phone = "+79998887766";
            var user = new User(email, phone, name, hash);

            _client.Users.Add(user);
            await _client.SaveChangesAsync();

            var newPhone = "+79998886677";
            var newEmail = "new@email.com";

            var sameNameUser = new User(newEmail, newPhone, name, hash);

            _client.Users.Add(sameNameUser);
            var func = async () => await _client.SaveChangesAsync();
            await func.Should().ThrowAsync<DbUpdateException>();
        }

        [Fact]
        async public Task DeleteUser_ValidData_DeleteAccount()
        {
            var hash = "10a6e6cc8311a3e2bcc09bf6c199adecd5dd59408c343e926b129c4914f3cb01";
            var email = "test@email.com";
            var name = "test_user";
            var phone = "+79998887766";
            var user = new User(email, phone, name, hash);

            var firstAccount = new Account(user.Id);
            var secondAccount = new Account(user.Id);

            _client.Users.Add(user);
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
