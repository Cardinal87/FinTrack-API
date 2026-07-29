using FinTrack.API;
using FinTrack.API.Core.Entities;
using FinTrack.IntegrationTests.Common;
using FluentAssertions;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using FinTrack.API.Infrastructure.Common.DTO;

namespace FinTrack.IntegrationTests.API.TransactionControllerTests
{
    public class GetTransactionsByIntervalTests : IClassFixture<FinTrackWebApplicationFactory<Program>>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly FinTrackWebApplicationFactory<Program> _factory;
        private readonly CancellationToken ct = TestContext.Current.CancellationToken;
        private readonly User _admin;
        private readonly User _user;

        private readonly List<Transaction> _transactions;
        public GetTransactionsByIntervalTests(FinTrackWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _factory = factory;
            (_user, _admin) = _factory.CreateBaseUsers();

            //Accounts
            var firstUserAccount = new Account(_user.Id);
            var secondUserAccount = new Account(_user.Id);
            var firtsAdminAccount = new Account(_admin.Id);
            var secondAdminAccount = new Account(_admin.Id);
            firtsAdminAccount.TopUp(500);
            firstUserAccount.TopUp(500);

            _factory.AccountRepositoryMock.AddAsync(firstUserAccount);
            _factory.AccountRepositoryMock.AddAsync(secondUserAccount);
            _factory.AccountRepositoryMock.AddAsync(firtsAdminAccount);
            _factory.AccountRepositoryMock.AddAsync(secondAdminAccount);

            //Transactions
            var firstUserTransaction = new Transaction(300, firstUserAccount.Id, secondAdminAccount.Id, new DateTime(2020, 6, 5, 4, 3, 2));
            var secondUserTransaction = new Transaction(300, secondUserAccount.Id, firstUserAccount.Id, new DateTime(2020, 5, 4, 3, 2, 1));

            var firstAdminTransaction = new Transaction(500, secondAdminAccount.Id, firstUserAccount.Id, new DateTime(2020, 6, 5, 3, 2, 1));
            var secondAdminTransaction = new Transaction(500, secondAdminAccount.Id, firstAdminTransaction.Id, new DateTime(2020, 6, 5, 1, 1, 1));


            firstUserAccount.AddOutgoingTransaction(firstUserTransaction);
            firstUserAccount.AddIncomingTransaction(secondUserTransaction);
            firstUserAccount.AddIncomingTransaction(firstAdminTransaction);


            _factory.TransactionRepositoryMock.AddAsync(firstUserTransaction);
            _factory.TransactionRepositoryMock.AddAsync(secondUserTransaction);
            _factory.TransactionRepositoryMock.AddAsync(firstAdminTransaction);
            _factory.TransactionRepositoryMock.AddAsync(secondAdminTransaction);
            _transactions = [firstUserTransaction, secondUserTransaction, firstAdminTransaction, secondAdminTransaction];

        }

        public void Dispose()
        {
            _factory.ResetMocks();
        }




        [Fact]
        async public Task GetTransactionByInterval_Admin_RecieveAll()
        {
            var token = await AuthHelper.GetTokenAsync(_client, _factory.MessagePublisherMock, _admin.Email, "pwd", _admin.Id);

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/transactions/interval?start=2010-06-06&end=2030-06-06");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);


            var response = await _client.SendAsync(httpRequest, ct);


            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var data = await response.Content.ReadFromJsonAsync<Dictionary<string, List<TransactionDb>>>(ct);
            data.Should().NotBeNull();

            var transactions = data["transactions"];
            transactions.Should().NotBeNull();
            transactions.Should().HaveCount(4);
        }

        [Fact]
        async public Task GetTransactionByInterval_CommonUser_ReceiveOnlyOwn()
        {
            var token = await AuthHelper.GetTokenAsync(_client, _factory.MessagePublisherMock, _user.Email, "pwd", _user.Id);

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/transactions/interval?start=2010-06-06&end=2030-06-06");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);


            var response = await _client.SendAsync(httpRequest, ct);


            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var data = await response.Content.ReadFromJsonAsync<Dictionary<string, List<TransactionDb>>>(ct);
            data.Should().NotBeNull();

            var transactions = data["transactions"];
            transactions.Should().NotBeNull();
            transactions.Should().Contain(t => t.Id == _transactions[0].Id);
            transactions.Should().Contain(t => t.Id == _transactions[1].Id);
            transactions.Should().Contain(t => t.Id == _transactions[2].Id);
        }
    }
}
