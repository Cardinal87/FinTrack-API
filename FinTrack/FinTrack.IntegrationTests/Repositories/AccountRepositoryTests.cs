using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Interfaces;
using FinTrack.API.Infrastructure.Data.Repositories;
using FinTrack.IntegrationTests.Common;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
namespace FinTrack.IntegrationTests.Repositories
{
    public class AccountRepositoryTests : DatabaseTestBase
    {
        private IAccountRepository _accountRepository = null!;
        private User defaultUser = null!;
        
        override async public Task InitializeAsync()
        {
            await base.InitializeAsync();
            _accountRepository = new AccountRepository(_client);
            var hash = "10a6e6cc8311a3e2bcc09bf6c199adecd5dd59408c343e926b129c4914f3cb01";
            var email = "test@email.com";
            var name = "test_user";
            var phone = "+79998887766";
            var user = new User(email, phone, name, hash);
            _client.Users.Add(user);
            await _client.SaveChangesAsync();
            defaultUser = user;
        }

        [Fact]
        async public Task AddAccount_ValidData_Success()
        {
            var first_account = new Account(defaultUser.Id);
            var second_account = new Account(defaultUser.Id);

            _accountRepository.Add(first_account);
            _accountRepository.Add(second_account);
            await _accountRepository.SaveChangesAsync();


            await _client.SaveChangesAsync();
            var accounts = await _client.Accounts.ToListAsync();

            accounts.Should().HaveCount(2);
            accounts.Should().Contain(first_account);
            accounts.Should().Contain(second_account);
        }

        [Fact]
        async public Task UpdateAccount_ValidData_Success()
        {
            var list = await AddValidAccounts(1);
            list[0].TopUp(500);

            await _accountRepository.UpdateAsync(list[0]);
            await _accountRepository.SaveChangesAsync();

            var updated = await _client.Accounts.FirstOrDefaultAsync(t => t.Id == list[0].Id);

            updated.Should().NotBeNull();
            updated.Balance.Should().Be(500);

        }

        [Fact]
        async public Task GetAccountById_ValidData_Success()
        {
            var list = await AddValidAccounts(1);

            var account = await _accountRepository.GetByIdAsync(list[0].Id);

            account.Should().NotBeNull();
            account.Id.Should().Be(list[0].Id);
            account.UserId.Should().Be(list[0].UserId);
        }

        [Fact]
        async public Task GetAllAccounts_ValidData_Success()
        {
            var list = await AddValidAccounts(3);

            var accounts = await _accountRepository.GetAllAsync();

            accounts.Should().HaveCount(3);
            accounts.Should().Contain(list[0]);
            accounts.Should().Contain(list[1]);
            accounts.Should().Contain(list[2]);
        }

        [Fact]
        async public Task DeleteAccount_ValidData_Success()
        {
            var list = await AddValidAccounts(2);

            await _accountRepository.DeleteAsync(list[0].Id);
            await _accountRepository.SaveChangesAsync();

            var accounts = await _client.Accounts.ToListAsync();

            accounts.Should().HaveCount(1);
            accounts.Should().Contain(list[1]);
        }

        private async Task<List<Account>> AddValidAccounts(int amount)
        {
            var accounts = new List<Account>();

            for (int i = 0; i < amount; i++)
            {
                var account = new Account(defaultUser.Id);
                accounts.Add(account);
                _client.Accounts.Add(account);
            }
            await _client.SaveChangesAsync();
            return accounts;
        }
    }

}
