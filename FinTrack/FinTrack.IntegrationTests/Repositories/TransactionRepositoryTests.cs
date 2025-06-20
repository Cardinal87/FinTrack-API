using AutoMapper;
using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Interfaces;
using FinTrack.API.Infrastructure.Data.DbEntities;
using FinTrack.API.Infrastructure.Data.Repositories;
using FinTrack.API.Infrastructure.Mappers;
using FinTrack.IntegrationTests.Common;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.IntegrationTests.Repositories
{
    public class TransactionRepositoryTests : DatabaseTestBase
    {
        private ITransactionRepository _transactionRepository = null!;
        private Account _fromAccount = null!;
        private Account _toAccount = null!;
        override async public Task InitializeAsync()
        {
            await base.InitializeAsync();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<TransactionMapper>();
            });

            IMapper mapper = config.CreateMapper();

            _transactionRepository = new TransactionRepository(_client, mapper);
            var user = new UserDb()
            {
                Email = "test@email.com",
                Phone = "+79998887766",
                Name = "test_user",
                PasswordHash = "10a6e6cc8311a3e2bcc09bf6c199adecd5dd59408c343e926b129c4914f3cb01"
            };

            _client.Users.Add(user);
            await _client.SaveChangesAsync();

            var fromAccount = new Account(user.Id);
            var toAccount = new Account(user.Id);

            var fromAccountDb = new AccountDb()
            {
                Id = fromAccount.Id,
                Balance = 0,
                UserId = user.Id
            };
            var toAccountDb = new AccountDb()
            {
                Id = toAccount.Id,
                Balance = 0,
                UserId = user.Id
            };

            _client.Accounts.Add(fromAccountDb);
            _client.Accounts.Add(toAccountDb);
            await _client.SaveChangesAsync();
            _fromAccount = fromAccount;
            _toAccount = toAccount;
        }

        [Fact]
        async public Task AddTransaction_ValidData_Success()
        {
            var transaction = new Transaction(300, _fromAccount.Id, _toAccount.Id, DateTime.UtcNow);

            _transactionRepository.Add(transaction);
            await _client.SaveChangesAsync();

            var list = await _client.Transactions.ToListAsync();
            list.Should().HaveCount(1);
            list[0].FromAccountId.Should().Be(_fromAccount.Id);
            list[0].ToAccountId.Should().Be(_toAccount.Id);
        }
        [Fact]
        async public Task GetTransactionById_ValidData_Success()
        {
            var list = await AddValidTransactionsWithDates(new List<DateTime>() 
            {
                new DateTime(2010, 10, 10, 5, 5, 5, 5, DateTimeKind.Utc)
            });
            
            var account = await _transactionRepository.GetByIdAsync(list[0].Id);

            account.Should().NotBeNull();
            account.Should().Be(list[0]);
        }

        [Fact]
        async public Task GetAllTransactions_ValidData_Success()
        {
            var list = await AddValidTransactionsWithDates(new List<DateTime>()
            {
                new DateTime(2010, 10, 10, 5, 5, 5, 5, DateTimeKind.Utc),
                new DateTime(2010, 10, 10, 15, 15, 15,15, DateTimeKind.Utc),
                new DateTime(2010, 10, 11, 15, 15, 15, 15, DateTimeKind.Utc)
            });

            var enumerable = await _transactionRepository.GetAllAsync();
            var transactionList = enumerable.ToList();

            transactionList.Should().HaveCount(3);
            transactionList.Should().Contain(list[0]);
            transactionList.Should().Contain(list[1]);
            transactionList.Should().Contain(list[2]);
        }

        [Fact]
        async public Task GetTransactionsByDate_ValidData_Success()
        {
            var list = await AddValidTransactionsWithDates(new List<DateTime>() 
            { 
                new DateTime(2010, 10, 10, 5, 5, 5, 5, DateTimeKind.Utc),
                new DateTime(2010, 10, 10, 15, 15, 15,15, DateTimeKind.Utc),
                new DateTime(2010, 10, 11, 15, 15, 15, 15, DateTimeKind.Utc)
            });

            var enumerable = await _transactionRepository.GetByDateAsync(new DateTime(2010, 10, 10, 10, 10, 10, 10, DateTimeKind.Utc));
            var transactionList = enumerable.ToList();

            transactionList.Should().HaveCount(2);
            transactionList.Should().Contain(list[0]);
            transactionList.Should().Contain(list[1]);
        }

        [Fact]
        async public Task GetTransactionsByInterval_ValidData_Success()
        {
            var list = await AddValidTransactionsWithDates(new List<DateTime>()
            {
                new DateTime(2012, 10, 10, 5, 5, 5, DateTimeKind.Utc),
                new DateTime(2011, 10, 10, 15, 15, 15, DateTimeKind.Utc),
                new DateTime(2017, 10, 10, 15, 15, 15, DateTimeKind.Utc)
            });

            var enumerable = await _transactionRepository.GetFromToDateAsync(
                new DateTime(2010, 10, 10, 10, 10, 10, DateTimeKind.Utc),
                new DateTime(2015, 10, 10, 10, 10, 10, DateTimeKind.Utc));
            var transactionList = enumerable.ToList();

            transactionList.Should().HaveCount(2);
            transactionList.Should().Contain(list[0]);
            transactionList.Should().Contain(list[1]);
        }

        async private Task<List<Transaction>> AddValidTransactionsWithDates(List<DateTime> dates)
        {
            var list = new List<Transaction>();

            foreach(var date in dates)
            {
                var transaction = new Transaction(100, _fromAccount.Id, _toAccount.Id, date);

                var transactionDb = new TransactionDb()
                {
                    Id = transaction.Id,
                    Amount = transaction.Amount,
                    FromAccountId = transaction.FromAccountId,
                    ToAccountId = transaction.ToAccountId,
                    Date = transaction.Date
                };
                _client.Transactions.Add(transactionDb);
                list.Add(transaction);
            }
            await _client.SaveChangesAsync();

            return list;
        }

    }
}
