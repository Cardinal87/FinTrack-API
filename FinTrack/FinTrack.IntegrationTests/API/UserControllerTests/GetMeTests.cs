using FinTrack.API;
using FinTrack.API.Core.Entities;
using FinTrack.IntegrationTests.Common;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FinTrack.IntegrationTests.API.UserControllerTests
{
    public class GetMeTests : IClassFixture<FinTrackWebApplicationFactory<Program>>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly FinTrackWebApplicationFactory<Program> _factory;
        private readonly User _user;


        public GetMeTests(FinTrackWebApplicationFactory<Program> factory)
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
        async public Task GetMe_ValidToken_Return200()
        {
            var token = await AuthHelper.GetToken(_client, _user.Email, "pwd");

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, "/api/users/me");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(httpRequest);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var data = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();

            data.Should().NotBeNullOrEmpty();
            data["email"].Should().Be(_user.Email);
            data["name"].Should().Be(_user.Name);
            data["phone"].Should().Be(_user.Phone);
            data.Keys.Should().NotContain("hash");
        }

        [Fact]
        async public Task GetMe_WithoutToken_Return401()
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, "/api/users/me");

            var response = await _client.SendAsync(httpRequest);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}