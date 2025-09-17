using FinTrack.API;
using FinTrack.API.Core.Entities;
using FinTrack.IntegrationTests.Common;
using FluentAssertions;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;

namespace FinTrack.IntegrationTests.API.TransactionControllerTests
{
    public class GetTransactionByIdTests : IClassFixture<FinTrackWebApplicationFactory<Program>>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly FinTrackWebApplicationFactory<Program> _factory;
        private readonly User _admin;
        private readonly User _user;

        private readonly List<Transaction> _transactions;
        public GetTransactionByIdTests(FinTrackWebApplicationFactory<Program> factory)
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

            _factory.AccountRepositoryMock.Add(firstUserAccount);
            _factory.AccountRepositoryMock.Add(secondUserAccount);
            _factory.AccountRepositoryMock.Add(firtsAdminAccount);
            _factory.AccountRepositoryMock.Add(secondAdminAccount);

            //Transactions
            var firstUserTransaction = new Transaction(300, firstUserAccount.Id, secondAdminAccount.Id, new DateTime(2020, 6, 5, 4, 3, 2));
            var secondUserTransaction = new Transaction(300, secondUserAccount.Id, firstUserAccount.Id, new DateTime(2020, 5, 4, 3, 2, 1));

            var firstAdminTransaction = new Transaction(500, secondAdminAccount.Id, firstUserAccount.Id, new DateTime(2020, 6, 5, 3, 2, 1));
            var secondAdminTransaction = new Transaction(500, secondAdminAccount.Id, firstAdminTransaction.Id, new DateTime(2020, 6, 5, 1, 1, 1));


            firstUserAccount.AddOutgoingTransaction(firstUserTransaction);
            firstUserAccount.AddIncomingTransaction(secondUserTransaction);
            firstUserAccount.AddIncomingTransaction(firstAdminTransaction);


            _factory.TransactionRepositoryMock.Add(firstUserTransaction);
            _factory.TransactionRepositoryMock.Add(secondUserTransaction);
            _factory.TransactionRepositoryMock.Add(firstAdminTransaction);
            _factory.TransactionRepositoryMock.Add(secondAdminTransaction);
            _transactions = [firstUserTransaction, secondUserTransaction, firstAdminTransaction, secondAdminTransaction];

        }

        public void Dispose()
        {
            _factory.ResetMocks();
        }


        [Fact]
        async public Task GetTransactionById_User_OwnTransaction_Return200()
        {
            var token = await AuthHelper.GetToken(_client, _user.Email, "pwd");

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/transactions/{_transactions[0].Id}");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);


            var response = await _client.SendAsync(httpRequest);


            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var data = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
            data.Should().NotBeNullOrEmpty();

            data["id"].Should().NotBeNull();
            data["source_account_id"].Should().NotBeNull();
            data["destination_account_id"].Should().NotBeNull();
            data["amount"].Should().NotBeNull();
        }

        [Fact]
        async public Task GetTransactionById_User_AnyTransaction_Return403()
        {
            var token = await AuthHelper.GetToken(_client, _user.Email, "pwd");

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/transactions/{_transactions[3].Id}");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);


            var response = await _client.SendAsync(httpRequest);


            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        async public Task GetTransactionById_Admin_AnyTransaction_Return200()
        {
            var token = await AuthHelper.GetToken(_client, _admin.Email, "pwd");

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/transactions/{_transactions[0].Id}");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);


            var response = await _client.SendAsync(httpRequest);


            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var data = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
            data.Should().NotBeNullOrEmpty();

            data["id"].Should().NotBeNull();
            data["source_account_id"].Should().NotBeNull();
            data["destination_account_id"].Should().NotBeNull();
            data["amount"].Should().NotBeNull();
        }

        [Fact]
        async public Task GetTransactionById_WithNonexistentId_Return200()
        {
            var token = await AuthHelper.GetToken(_client, _admin.Email, "pwd");

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/transactions/{Guid.NewGuid()}");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);


            var response = await _client.SendAsync(httpRequest);


            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}