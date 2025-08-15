using FinTrack.API;
using FinTrack.API.Core.Common;
using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Interfaces;
using FinTrack.API.DTO;
using FinTrack.API.TestMocks.Builders;
using FinTrack.IntegrationTests.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FinTrack.IntegrationTests.API
{
    public class TokenControllerTests : IClassFixture<FinTrackWebApplicationFactory<Program>>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly FinTrackWebApplicationFactory<Program> _factory;

        public TokenControllerTests(FinTrackWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _factory = factory;
            using (var scope = factory.Services.CreateScope())
            {
                var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

                var user = new UserBuilder().WithPassword("pwd", hasher)
                    .WithEmail("test@email.com")
                    .WithRoles(UserRoles.Admin, UserRoles.User)
                    .Build();

                _factory.UserRepositoryMock.Add(user);
            }
        }
        [Fact]
        async public Task GetJwtToken_ValidCredentials_Returns200()
        {
            var request = new LoginRequest
            {
                Login = "test@email.com",
                Password = "pwd",
            };

            var response = await _client.PostAsJsonAsync("/api/auth/token", request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var data = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            data.Should().NotBeNullOrEmpty();
            data["token"].Should().NotBeNullOrEmpty();
        }

        [Fact]
        async public Task GetJwtToken_MissingPassword_Returns400()
        {
            var request = new LoginRequest
            {
                Login = "test@email.com",
            };

            var response = await _client.PostAsJsonAsync("/api/auth/token", request);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        async public Task GetJwtToken_MissingLogin_Returns400()
        {
            var request = new LoginRequest
            {
                Password = "pwd",
            };

            var response = await _client.PostAsJsonAsync("/api/auth/token", request);

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

            var response = await _client.PostAsJsonAsync("/api/auth/token", request);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        async public Task GetJwtStatus_ValidToken_Returns200()
        {
            var request = new LoginRequest
            {
                Login = "test@email.com",
                Password = "pwd",
            };
            var token = await AuthHelper.GetToken(_client, request);
            
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, "/api/auth/token/status");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var response = await _client.SendAsync(httpRequest);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        async public Task GetJwtStatus_MissingToken_Returns401()
        {
            var response = await _client.GetAsync("/api/auth/token/status");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        public void Dispose()
        {
            _factory.ResetMocks();
        }
    }
}
