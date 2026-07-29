using FinTrack.API;
using FinTrack.API.Core.Entities;
using FinTrack.IntegrationTests.Common;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FinTrack.IntegrationTests.API.UserControllerTests
{
    public class GetUserByIdTests : IClassFixture<FinTrackWebApplicationFactory<Program>>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly FinTrackWebApplicationFactory<Program> _factory;
        private readonly CancellationToken ct = TestContext.Current.CancellationToken;
        private readonly User _admin;
        private readonly User _user;


        public GetUserByIdTests(FinTrackWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _factory = factory;
            (_user, _admin) = _factory.CreateBaseUsers();
        }

        public void Dispose()
        {
            _factory.ResetMocks();
        }




        [Fact]
        async public Task GetUserById_WithAdminToken_Return200()
        {
            var token = await AuthHelper.GetTokenAsync(_client, _factory.MessagePublisherMock, _admin.Email, "pwd", _admin.Id);

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/users/{_user.Id}");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(httpRequest, ct);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var data = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>(ct);

            data.Should().NotBeNullOrEmpty();
            data["email"].Should().Be(_user.Email);
            data["name"].Should().Be(_user.Name);
            data["phone"].Should().Be(_user.Phone);
            data["hash"].Should().Be(_user.PasswordHash);
        }

        [Fact]
        async public Task GetUserById_RandomGuid_Return404()
        {
            var token = await AuthHelper.GetTokenAsync(_client, _factory.MessagePublisherMock, _admin.Email, "pwd", _admin.Id);

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/users/{Guid.NewGuid()}");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(httpRequest, ct);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        async public Task GetUserById_WithSimpleUserToken_Return403()
        {
            var token = await AuthHelper.GetTokenAsync(_client, _factory.MessagePublisherMock, _user.Email, "pwd", _user.Id);

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/users/{_admin.Id}");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(httpRequest, ct);

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        async public Task GetUserById_WithoutToken_Return401()
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/users/{_user.Id}");

            var response = await _client.SendAsync(httpRequest, ct  );

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}