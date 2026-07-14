using FinTrack.API;
using FinTrack.API.Core.Entities;
using FinTrack.IntegrationTests.Common;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FinTrack.IntegrationTests.API.AccountControllerTests
{
    public class GetAccountByIdTests : IClassFixture<FinTrackWebApplicationFactory<Program>>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly FinTrackWebApplicationFactory<Program> _factory;
        private readonly User _admin;
        private readonly User _user;

        private readonly Account _userAccount;
        private readonly Account _adminAccount;

        public GetAccountByIdTests(FinTrackWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _factory = factory;
            (_user, _admin) = _factory.CreateBaseUsers();

            var userAccount = new Account(_user.Id);
            var adminAccount = new Account(_admin.Id);

            _factory.AccountRepositoryMock.AddAsync(userAccount);
            _factory.AccountRepositoryMock.AddAsync(adminAccount);
            _userAccount = userAccount;
            _adminAccount = adminAccount;
        }

        public void Dispose()
        {
            _factory.ResetMocks();
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
    }
}