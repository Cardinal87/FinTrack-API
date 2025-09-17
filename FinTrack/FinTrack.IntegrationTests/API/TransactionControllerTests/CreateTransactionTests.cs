using FinTrack.API;
using FinTrack.API.Core.Entities;
using FinTrack.API.DTO;
using FinTrack.IntegrationTests.Common;
using FluentAssertions;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;

namespace FinTrack.IntegrationTests.API.TransactionControllerTests
{
    public class CreateTransactionTests : IClassFixture<FinTrackWebApplicationFactory<Program>>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly FinTrackWebApplicationFactory<Program> _factory;
        private readonly User _admin;
        private readonly User _user;

        private readonly List<Account> _userAccounts;
        private readonly List<Account> _adminAccounts;

        public CreateTransactionTests(FinTrackWebApplicationFactory<Program> factory)
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


            _factory.TransactionRepositoryMock.Add(firstUserTransaction);
            _factory.TransactionRepositoryMock.Add(secondUserTransaction);
            _factory.TransactionRepositoryMock.Add(firstAdminTransaction);
            _factory.TransactionRepositoryMock.Add(secondAdminTransaction);

        }

        public void Dispose()
        {
            _factory.ResetMocks();
        }

        [Fact]
        async public Task CreateTransaction_FromOwnAccount_Return201()
        {
            var token = await AuthHelper.GetToken(_client, _user.Email, "pwd");
            var request = new CreateTransactionRequest()
            {
                SourceAccountId = _userAccounts[0].Id,
                DestinationAccountId = _adminAccounts[1].Id,
                Amount = 500
            };
            var content = JsonContent.Create(request);

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/transactions");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            httpRequest.Content = content;


            var response = await _client.SendAsync(httpRequest);


            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var data = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
            data.Should().NotBeNullOrEmpty();
            data["id"].Should().NotBeNull();

            _userAccounts[0].Balance.Should().Be(0);
            _adminAccounts[1].Balance.Should().Be(500);
        }

        [Fact]
        async public Task CreateTransaction_InsufficientFunds_Return400()
        {
            var token = await AuthHelper.GetToken(_client, _user.Email, "pwd");
            var request = new CreateTransactionRequest()
            {
                SourceAccountId = _userAccounts[0].Id,
                DestinationAccountId = _adminAccounts[1].Id,
                Amount = 1000
            };
            var content = JsonContent.Create(request);

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/transactions");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            httpRequest.Content = content;


            var response = await _client.SendAsync(httpRequest);


            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        async public Task CreateTransaction_FromAnyAccount_Return403()
        {
            var token = await AuthHelper.GetToken(_client, _user.Email, "pwd");
            var request = new CreateTransactionRequest()
            {
                SourceAccountId = _adminAccounts[0].Id,
                DestinationAccountId = _userAccounts[1].Id,
                Amount = 500
            };
            var content = JsonContent.Create(request);

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/transactions");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            httpRequest.Content = content;


            var response = await _client.SendAsync(httpRequest);


            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        async public Task CreateTransaction_ForNonExistentUser_Return401()
        {
            var token = await AuthHelper.GetToken(_client, _user.Email, "pwd");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/transaction");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            await _factory.UserRepositoryMock.DeleteAsync(_user.Id);


            var response = await _client.SendAsync(httpRequest);


            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}