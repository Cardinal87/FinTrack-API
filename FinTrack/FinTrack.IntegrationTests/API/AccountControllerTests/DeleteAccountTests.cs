using FinTrack.API;
using FinTrack.API.Core.Entities;
using FinTrack.IntegrationTests.Common;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;

namespace FinTrack.IntegrationTests.API.AccountControllerTests
{
    public class DeleteAccountTests : IClassFixture<FinTrackWebApplicationFactory<Program>>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly FinTrackWebApplicationFactory<Program> _factory;
        private readonly CancellationToken ct = TestContext.Current.CancellationToken;
        private readonly User _admin;
        private readonly User _user;

        private readonly Account _userAccount;
        private readonly Account _adminAccount;

        public DeleteAccountTests(FinTrackWebApplicationFactory<Program> factory)
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
        async public Task DeleteAccount_User_OwnAccount_Return204()
        {
            var token = await AuthHelper.GetTokenAsync(_client, _factory.MessagePublisherMock, _user.Email, "pwd", _user.Id);

            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/accounts/{_userAccount.Id}");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(httpRequest, ct);

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var deletedAccount = await _factory.AccountRepositoryMock.GetByIdAsync(_userAccount.Id);
            deletedAccount.Should().BeNull();
        }

        [Fact]
        async public Task DeleteAccount_User_AnyAccount_Return403()
        {
            var token = await AuthHelper.GetTokenAsync(_client, _factory.MessagePublisherMock, _user.Email, "pwd", _user.Id);

            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/accounts/{_adminAccount.Id}");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(httpRequest, ct);

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        async public Task DeleteAccount_Admin_AnyAccount_Return204()
        {
            var token = await AuthHelper.GetTokenAsync(_client, _factory.MessagePublisherMock, _admin.Email, "pwd", _admin.Id);

            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/accounts/{_userAccount.Id}");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(httpRequest, ct);

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var deletedAccount = await _factory.AccountRepositoryMock.GetByIdAsync(_userAccount.Id);
            deletedAccount.Should().BeNull();
        }

        [Fact]
        async public Task DeleteAccount_WithNonexistentId_Return404()
        {
            var token = await AuthHelper.GetTokenAsync(_client, _factory.MessagePublisherMock, _admin.Email, "pwd", _admin.Id);

            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/accounts/{Guid.NewGuid()}");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(httpRequest, ct);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}