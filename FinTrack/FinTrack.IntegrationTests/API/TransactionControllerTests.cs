
using FinTrack.API;
using FinTrack.API.Application.UseCases.Transactions.CreateTransaction;
using FinTrack.API.Core.Entities;
using FinTrack.API.DTO;
using FinTrack.IntegrationTests.Common;
using FluentAssertions;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using FinTrack.API.Infrastructure.Data.DbEntities;

namespace FinTrack.IntegrationTests.API
{
    public class TransactionControllerTests : IClassFixture<FinTrackWebApplicationFactory<Program>>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly FinTrackWebApplicationFactory<Program> _factory;
        private readonly User _admin;
        private readonly User _user;

        private readonly List<Account> _userAccounts;
        private readonly List<Account> _adminAccounts;

        private readonly List<Transaction> _transactions;
        public TransactionControllerTests(FinTrackWebApplicationFactory<Program> factory)
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
            _userAccounts = [firstUserAccount,  secondUserAccount];
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
            _transactions = [firstUserTransaction, secondUserTransaction, firstAdminTransaction, secondAdminTransaction];

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

        [Fact]
        async public Task GetTransactionByDate_Admin_ReceiveAll()
        {
            var token = await AuthHelper.GetToken(_client, _admin.Email, "pwd");

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/transactions/date/2020-06-05");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);


            var response = await _client.SendAsync(httpRequest);


            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var data = await response.Content.ReadFromJsonAsync<Dictionary<string, List<TransactionDb>>>();
            data.Should().NotBeNull();

            var transactions = data["transactions"];

            transactions.Should().NotBeNull();
            transactions.Should().HaveCount(3);
            transactions.Should().Contain(t => t.Id == _transactions[0].Id);
            transactions.Should().Contain(t => t.Id == _transactions[2].Id);
            transactions.Should().Contain(t => t.Id == _transactions[3].Id);
        }

        [Fact]
        async public Task GetTransactionByDate_CommonUser_ReceiveOnlyOwn()
        {
            var token = await AuthHelper.GetToken(_client, _user.Email, "pwd");

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/transactions/date/2020-06-05");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);


            var response = await _client.SendAsync(httpRequest);


            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var data = await response.Content.ReadFromJsonAsync<Dictionary<string, List<TransactionDb>>>();
            data.Should().NotBeNull();

            var transactions = data["transactions"];

            transactions.Should().NotBeNull();
            transactions.Should().HaveCount(2);
            transactions.Should().Contain(t => t.Id == _transactions[0].Id);
            transactions.Should().Contain(t => t.Id == _transactions[2].Id);
        }


        [Fact]
        async public Task GetTransactionByInterval_Admin_RecieveAll()
        {
            var token = await AuthHelper.GetToken(_client, _admin.Email, "pwd");

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/transactions/interval?start=2010-06-06&end=2030-06-06");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);


            var response = await _client.SendAsync(httpRequest);


            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var data = await response.Content.ReadFromJsonAsync<Dictionary<string,List<TransactionDb>>>();
            data.Should().NotBeNull();

            var transactions = data["transactions"];
            transactions.Should().NotBeNull();
            transactions.Should().HaveCount(4);
        }

        [Fact]
        async public Task GetTransactionByInterval_CommonUser_ReceiveOnlyOwn()
        {
            var token = await AuthHelper.GetToken(_client, _user.Email, "pwd");

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/transactions/interval?start=2010-06-06&end=2030-06-06");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);


            var response = await _client.SendAsync(httpRequest);


            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var data = await response.Content.ReadFromJsonAsync<Dictionary<string, List<TransactionDb>>>();
            data.Should().NotBeNull();

            var transactions = data["transactions"];
            transactions.Should().NotBeNull();
            transactions.Should().Contain(t => t.Id == _transactions[0].Id);
            transactions.Should().Contain(t => t.Id == _transactions[1].Id);
            transactions.Should().Contain(t => t.Id == _transactions[2].Id);
        }
    }
}
