using FinTrack.API;
using FinTrack.API.Core.Common;
using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Interfaces;
using FinTrack.API.DTO;
using FinTrack.IntegrationTests.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FinTrack.IntegrationTests.API
{
    public class TokenControllerTests : IClassFixture<FinTrackWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly FinTrackWebApplicationFactory<Program> _factory;

        public TokenControllerTests(FinTrackWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _factory = factory;
            using (var scope = factory.Services.CreateScope())
            {
                var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

                var hash = hasher.GetHash("pwd");
                var email = "test@email.com";
                var name = "test_user";
                var phone = "+79998887766";
                var user = new User(email, phone, name, hash);
                user.AssignRole(UserRoles.Admin);
                user.AssignRole(UserRoles.User);

                userRepo.Add(user);
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
                Password = "password",
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

        
    }
}
