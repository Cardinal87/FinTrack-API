
using FinTrack.API;
using FinTrack.API.Core.Common;
using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Interfaces;
using FinTrack.API.DTO;
using FinTrack.API.TestMocks.Builders;
using FinTrack.IntegrationTests.Common;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FinTrack.IntegrationTests.API
{
    public class AccountControllerTests : IClassFixture<FinTrackWebApplicationFactory<Program>>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly FinTrackWebApplicationFactory<Program> _factory;
        private readonly User _admin;
        private readonly User _user;

        private readonly Account _userAccount;
        private readonly Account _adminAccount;  

        public AccountControllerTests(FinTrackWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _factory = factory;
            (_user, _admin) = _factory.CreateBaseUsers();

            var userAccount = new Account(_user.Id);
            var adminAccount = new Account(_admin.Id);

            _factory.AccountRepositoryMock.Add(userAccount);
            _factory.AccountRepositoryMock.Add(adminAccount);
            _userAccount = userAccount;
            _adminAccount = adminAccount;

                
        }

        public void Dispose()
        {
            _factory.ResetMocks();
        }

        [Fact]
        async public Task CreateAccount_ValidData_Return201()
        {
            var token = await AuthHelper.GetToken(_client, _user.Email, "pwd");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/accounts");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);


            var response = await _client.SendAsync(httpRequest);


            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var data = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            data.Should().NotBeNullOrEmpty();

            var guid = Guid.Parse(data["id"]);
            var account = await _factory.AccountRepositoryMock.GetByIdAsync(guid);
            account.Should().NotBeNull();
        }

        [Fact]
        async public Task CreateAccount_WithoutToken_Return401()
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/accounts");

            var response = await _client.SendAsync(httpRequest);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        async public Task CreateAccount_ForNonExistentUser_Return404()
        {
            var token = await AuthHelper.GetToken(_client, _user.Email, "pwd");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/accounts");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            await _factory.UserRepositoryMock.DeleteAsync(_user.Id);


            var response = await _client.SendAsync(httpRequest);

            
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }


        [Fact]
        async public Task GetAccountById_User_OwnAccount_Return200()
        {
            var token = await AuthHelper.GetToken(_client, _user.Email, "pwd");

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/accounts/{_userAccount.Id}");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(httpRequest);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var data = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
            data.Should().NotBeNullOrEmpty();

            data["id"].Should().NotBeNull();
            data["balance"].Should().NotBeNull();
            data["user_id"].Should().NotBeNull();
        }

        [Fact]
        async public Task GetAccountById_User_AnyAccount_Return403()
        {
            var token = await AuthHelper.GetToken(_client, _user.Email, "pwd");

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/accounts/{_adminAccount.Id}");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(httpRequest);

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        async public Task GetAccountById_Admin_AnyAccount_Return200()
        {
            var token = await AuthHelper.GetToken(_client, _admin.Email, "pwd");

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/accounts/{_userAccount.Id}");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(httpRequest);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var data = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
            data.Should().NotBeNullOrEmpty();

            data["id"].Should().NotBeNull();
            data["balance"].Should().NotBeNull();
            data["user_id"].Should().NotBeNull();
        }

        [Fact]
        async public Task GetAccountById_WithNonexistentId_Return404()
        {
            var token = await AuthHelper.GetToken(_client, _admin.Email, "pwd");

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/accounts/{Guid.NewGuid()}");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(httpRequest);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        async public Task DeleteAccount_User_OwnAccount_Return204()
        {
            var token = await AuthHelper.GetToken(_client, _user.Email, "pwd");

            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/accounts/{_userAccount.Id}");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(httpRequest);

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var deletedAccount = await _factory.AccountRepositoryMock.GetByIdAsync(_userAccount.Id);
            deletedAccount.Should().BeNull();
        }

        [Fact]
        async public Task DeleteAccount_User_AnyAccount_Return403()
        {
            var token = await AuthHelper.GetToken(_client, _user.Email, "pwd");

            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/accounts/{_adminAccount.Id}");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(httpRequest);

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        async public Task DeleteAccount_Admin_AnyAccount_Return204()
        {
            var token = await AuthHelper.GetToken(_client, _admin.Email, "pwd");

            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/accounts/{_userAccount.Id}");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(httpRequest);

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var deletedAccount = await _factory.AccountRepositoryMock.GetByIdAsync(_userAccount.Id);
            deletedAccount.Should().BeNull();
        }

        [Fact]
        async public Task DeleteAccount_WithNonexistentId_Return404()
        {
            var token = await AuthHelper.GetToken(_client, _admin.Email, "pwd");

            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/accounts/{Guid.NewGuid()}");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(httpRequest);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        async public Task TopUpAccount_User_Return403()
        {
            var token = await AuthHelper.GetToken(_client, _user.Email, "pwd");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/accounts/{_userAccount.Id}/topup?amount=300");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(httpRequest);

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        async public Task DebitAccount_User_Return403()
        {
            var token = await AuthHelper.GetToken(_client, _user.Email, "pwd");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/accounts/{_userAccount.Id}/debit?amount=300");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(httpRequest);

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        async public Task TopUpAccount_Admin_Return200()
        {
            var token = await AuthHelper.GetToken(_client, _admin.Email, "pwd");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/accounts/{_userAccount.Id}/topup?amount=300");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(httpRequest);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var data = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();
            data.Should().NotBeNullOrEmpty();
            data["balance"].Should().Be(300);
            _userAccount.Balance.Should().Be(300);
        }

        [Fact]
        async public Task TopUpAccount_WithNonexistentId_Return200()
        {
            var token = await AuthHelper.GetToken(_client, _admin.Email, "pwd");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/accounts/{Guid.NewGuid()}/topup?amount=300");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(httpRequest);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        async public Task DebitAccount_Admin_Return200()
        {
            var token = await AuthHelper.GetToken(_client, _admin.Email, "pwd");
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/accounts/{_userAccount.Id}/debit?amount=300");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _userAccount.TopUp(300);


            var response = await _client.SendAsync(httpRequest);


            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var data = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();
            data.Should().NotBeNullOrEmpty();
            data["balance"].Should().Be(0);
            _userAccount.Balance.Should().Be(0);
        }

        [Fact]
        async public Task DebitAccount_InsufficientFunds_Return400()
        {
            var token = await AuthHelper.GetToken(_client, _admin.Email, "pwd");
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/accounts/{_userAccount.Id}/debit?amount=300");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);


            var response = await _client.SendAsync(httpRequest);


            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

    }
}
