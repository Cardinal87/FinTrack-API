using FinTrack.API;
using FinTrack.API.Core.Entities;
using FinTrack.IntegrationTests.Common;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FinTrack.IntegrationTests.API.AccountControllerTests
{
    public class TopUpAccountTests : IClassFixture<FinTrackWebApplicationFactory<Program>>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly FinTrackWebApplicationFactory<Program> _factory;
        private readonly User _admin;
        private readonly User _user;

        private readonly Account _userAccount;

        public TopUpAccountTests(FinTrackWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _factory = factory;
            (_user, _admin) = _factory.CreateBaseUsers();

            var userAccount = new Account(_user.Id);

            _factory.AccountRepositoryMock.Add(userAccount);
            _userAccount = userAccount;


        }

        public void Dispose()
        {
            _factory.ResetMocks();
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
    }
}