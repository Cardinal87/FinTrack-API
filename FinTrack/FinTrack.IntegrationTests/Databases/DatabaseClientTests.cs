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

            _client.Users.Add(user);
            await _client.SaveChangesAsync();
            Guid id = user.Account.Id;

            _client.Users.Remove(user);
            await _client.SaveChangesAsync();

            var account = await _client.Accounts.FirstOrDefaultAsync(t => t.Id == id);
            account.Should().BeNull();

        }

        [Fact]
        async public Task CreateUser_ValidData_CreateAccount()
        {
            var hash = "10a6e6cc8311a3e2bcc09bf6c199adecd5dd59408c343e926b129c4914f3cb01";
            var email = "test@email.com";
            var name = "test_user";
            var phone = "+79998887766";
            var user = new User(email, phone, name, hash);

            _client.Users.Add(user);
            await _client.SaveChangesAsync();
            Guid id = user.Account.Id;

            var account = await _client.Accounts.FirstOrDefaultAsync(t => t.Id == id);

            account.Should().NotBeNull();
            account.Balance.Should().Be(0);
            account.OutgoingTransactions.Should().BeEmpty();
            account.IncomingTransactions.Should().BeEmpty();
        }
    }
}
