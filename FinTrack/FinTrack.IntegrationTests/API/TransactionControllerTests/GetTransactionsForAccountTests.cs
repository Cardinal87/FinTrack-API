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
    public class GetTransactionsForAccountTests : IClassFixture<FinTrackWebApplicationFactory<Program>>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly FinTrackWebApplicationFactory<Program> _factory;
        private readonly User _admin;
        private readonly User _user;

        private readonly List<Account> _userAccounts;
        private readonly List<Account> _adminAccounts;

        public GetTransactionsForAccountTests(FinTrackWebApplicationFactory<Program> factory)
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
            _userAccounts = [firstUserAccount, secondUserAccount];
            _adminAccounts = [firtsAdminAccount, secondAdminAccount];

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

        }

        public void Dispose()
        {
            _factory.ResetMocks();
        }


        [Fact]
        async public Task GetTransactionsForAccount_User_OwnAccount_Return200()
        {
            var token = await AuthHelper.GetToken(_client, _user.Email, "pwd");

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/transactions/account/{_userAccounts[0].Id}");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);


            var response = await _client.SendAsync(httpRequest);


            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var data = await response.Content.ReadFromJsonAsync<Dictionary<string, List<TransactionDb>>>();
            data.Should().NotBeNull();

            var transactions = data["transactions"];

            transactions.Should().NotBeNull();
            transactions.Should().HaveCount(3);
        }

        [Fact]
        async public Task GetTransactionsForAccount_User_AnyAccount_Return403()
        {
            var token = await AuthHelper.GetToken(_client, _user.Email, "pwd");

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/transactions/account/{_adminAccounts[0].Id}");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);


            var response = await _client.SendAsync(httpRequest);


            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        async public Task GetTransactionsForAccount_Admin_AnyAccount_Return200()
        {
            var token = await AuthHelper.GetToken(_client, _admin.Email, "pwd");

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/transactions/account/{_userAccounts[0].Id}");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);


            var response = await _client.SendAsync(httpRequest);


            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var data = await response.Content.ReadFromJsonAsync<Dictionary<string, List<TransactionDb>>>();
            data.Should().NotBeNull();

            var transactions = data["transactions"];

            transactions.Should().NotBeNull();
            transactions.Should().HaveCount(3);
        }
    }
}