using FinTrack.API;
using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Interfaces;
using FinTrack.IntegrationTests.Common;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FinTrack.IntegrationTests.API.AccountControllerTests
{
    public class CreateAccountTests : IClassFixture<FinTrackWebApplicationFactory<Program>>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly FinTrackWebApplicationFactory<Program> _factory;
        private readonly CancellationToken ct = TestContext.Current.CancellationToken;
        private readonly User _user;


        public CreateAccountTests(FinTrackWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _factory = factory;
            (_user, _) = _factory.CreateBaseUsers();
        }

        public void Dispose()
        {
            _factory.ResetMocks();
        }

        [Fact]
        async public Task CreateAccount_ValidData_Return201()
        {
            var token = await AuthHelper.GetTokenAsync(_client, _factory.MessagePublisherMock, _user.Email, "pwd", _user.Id);

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/accounts");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);


            var response = await _client.SendAsync(httpRequest, ct);


            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var data = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>(ct);
            data.Should().NotBeNullOrEmpty();

            var guid = Guid.Parse(data["id"]);
            var account = await _factory.AccountRepositoryMock.GetByIdAsync(guid);
            account.Should().NotBeNull();
        }

        [Fact]
        async public Task CreateAccount_WithoutToken_Return401()
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/accounts");

            var response = await _client.SendAsync(httpRequest, ct);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        async public Task CreateAccount_ForNonExistentUser_Return401()
        {
            var token = await AuthHelper.GetTokenAsync(_client, _factory.MessagePublisherMock, _user.Email, "pwd", _user.Id);

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/accounts");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var rep = _factory.Services.GetRequiredService<IUserRepository>();
            var user = rep.DeleteAsync(_user.Id);

            var response = await _client.SendAsync(httpRequest, ct);


            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}