using FinTrack.API;
using FinTrack.API.Core.Common;
using FinTrack.API.Application.Interfaces;
using FinTrack.API.DTO;
using FinTrack.API.TestMocks.Builders;
using FinTrack.IntegrationTests.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using StackExchange.Redis;

namespace FinTrack.IntegrationTests.API
{
    public class TokenControllerTests : IClassFixture<FinTrackWebApplicationFactory<Program>>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly FinTrackWebApplicationFactory<Program> _factory;
        private readonly CancellationToken ct = TestContext.Current.CancellationToken;
        private readonly Guid _testUserId;

        public TokenControllerTests(FinTrackWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _factory = factory;
            using (var scope = factory.Services.CreateScope())
            {
                var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
                var totpService = scope.ServiceProvider.GetRequiredService<ITotpService>();

                var user = new UserBuilder().WithPassword("pwd", hasher)
                    .WithEmail("test@email.com")
                    .WithRoles(UserRoles.Admin, UserRoles.User)
                    .WithTotpSecret(totpService.GenerateSecret())
                    .WithVerifiedEmail()
                    .Build();

                _testUserId = user.Id;
                _factory.UserRepositoryMock.AddAsync(user);
            }
        }
        [Fact]
        async public Task GetJwtToken_WithTwoFactorAuth_Returns200()
        {
            var request = new LoginRequest
            {
                Login = "test@email.com",
                Password = "pwd",
            };

            var response = await _client.PostAsJsonAsync("/api/auth/token", request, ct);

            response.StatusCode.Should().Be(HttpStatusCode.Accepted);

            var data = await response.Content.ReadFromJsonAsync<JsonNode>(ct);
            data.Should().NotBeNull();
            data["challenge_token"].Should().NotBeNull();
            data["expires_in"]!.GetValue<int>().Should().BeGreaterThan(0);

            var code = _factory.MessagePublisherMock.GetCode(_testUserId);
            if (string.IsNullOrEmpty(code))
            {
                throw new InvalidOperationException("Fake publisher did not capture the TOTP code");
            }

            var challengeToken = (string)data["challenge_token"]!;
            var verifyRequest = new VerifyCodeRequest { Code = code };

            var verifyMessage = new HttpRequestMessage(HttpMethod.Post, "/api/auth/2fa/complete");
            verifyMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", challengeToken);
            verifyMessage.Content = JsonContent.Create(verifyRequest);

            var verifyResponse = await _client.SendAsync(verifyMessage, ct);

            verifyResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var verifyData = await verifyResponse.Content.ReadFromJsonAsync<JsonNode>(ct);

            verifyData.Should().NotBeNull();
            verifyData["access_token"].Should().NotBeNull();
            verifyData["refresh_token"].Should().NotBeNull();
            verifyData["expires_in"]!.GetValue<int>().Should().BeGreaterThan(0);
        }

        [Fact]
        async public Task GetJwtToken_MissingPassword_Returns400()
        {
            var request = new LoginRequest
            {
                Login = "test@email.com",
            };

            var response = await _client.PostAsJsonAsync("/api/auth/token", request, ct);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        async public Task GetJwtToken_MissingLogin_Returns400()
        {
            var request = new LoginRequest
            {
                Password = "pwd",
            };

            var response = await _client.PostAsJsonAsync("/api/auth/token", request, ct);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        async public Task GetJwtToken_InvalidPassword_Returns401()
        {
            var request = new LoginRequest
            {
                Login = "test@email.com",
                Password = "invalid"
            };

            var response = await _client.PostAsJsonAsync("/api/auth/token", request, ct);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        async public Task GetJwtToken_InvalidLogin_Returns401()
        {
            var request = new LoginRequest
            {
                Login = "invalid@email.com",
                Password = "pwd"
            };

            var response = await _client.PostAsJsonAsync("/api/auth/token", request, ct);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        async public Task GetJwtStatus_ValidToken_Returns200()
        {
            var token = await AuthHelper.GetTokenAsync(_client, _factory.MessagePublisherMock, "test@email.com", "pwd", _testUserId);
            
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, "/api/auth/token/status");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var response = await _client.SendAsync(httpRequest, ct);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        async public Task GetJwtStatus_MissingToken_Returns401()
        {
            var response = await _client.GetAsync("/api/auth/token/status", ct);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        public void Dispose()
        {
            _factory.ResetMocks();
        }
    }
}
