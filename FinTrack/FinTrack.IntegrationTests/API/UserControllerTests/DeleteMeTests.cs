using FinTrack.API;
using FinTrack.API.Core.Entities;
using FinTrack.IntegrationTests.Common;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;

namespace FinTrack.IntegrationTests.API.UserControllerTests
{
    public class DeleteMeTests : IClassFixture<FinTrackWebApplicationFactory<Program>>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly FinTrackWebApplicationFactory<Program> _factory;
        private readonly CancellationToken ct = TestContext.Current.CancellationToken;
        private readonly User _user;


        public DeleteMeTests(FinTrackWebApplicationFactory<Program> factory)
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
        async public Task DeleteMe_WithoutToken_Return401()
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/users/me");

            var response = await _client.SendAsync(httpRequest, ct);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        async public Task DeleteMe_WhenAlreadyDeleted_Return401()
        {
            var token = await AuthHelper.GetTokenAsync(_client, _factory.MessagePublisherMock,_user.Email, "pwd", _user.Id);
            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/users/me");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            await _client.SendAsync(httpRequest, ct);

            var nextRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/users/me");
            nextRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.SendAsync(nextRequest, ct);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }


    }
}
