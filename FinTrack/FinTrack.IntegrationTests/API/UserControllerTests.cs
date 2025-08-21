using FinTrack.API;
using FinTrack.API.Core.Entities;
using FinTrack.API.DTO;
using FinTrack.IntegrationTests.Common;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FinTrack.IntegrationTests.API
{
    public class UserControllerTests : IClassFixture<FinTrackWebApplicationFactory<Program>>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly FinTrackWebApplicationFactory<Program> _factory;
        private readonly User _admin;
        private readonly User _user;
        

        public UserControllerTests(FinTrackWebApplicationFactory<Program> factory)
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
        async public Task CreateUser_ValidData_Return201()
        {
            var request = new CreateUserRequest
            {
                Email = "newemail@gmail.com",
                Phone = "+76549874321",
                Password = "password",
                Name = "newname"
            };
            
            var response = await _client.PostAsJsonAsync("/api/users", request);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var data = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            
            data.Should().NotBeNullOrEmpty();
            data["id"].Should().NotBeNullOrEmpty();

            var createdUser = await _factory.UserRepositoryMock.GetByIdAsync(Guid.Parse(data["id"]));
            createdUser.Should().NotBeNull();
        }

        [Fact]
        async public Task CreateUser_InvalidRequest_Return400()
        {
            var request = new CreateUserRequest
            {
                Email = "newemail@gmail.com",
                Phone = "+76549874321",
                Name = "newname"
            };

            var response = await _client.PostAsJsonAsync("/api/users", request);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
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

        [Fact]
        async public Task GetUserById_WithAdminToken_Return200()
        {
            var token = await AuthHelper.GetToken(_client, _admin.Email, "pwd");

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/users/{_user.Id}");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(httpRequest);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var data = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();

            data.Should().NotBeNullOrEmpty();
            data["email"].Should().Be(_user.Email);
            data["name"].Should().Be(_user.Name);
            data["phone"].Should().Be(_user.Phone);
            data["hash"].Should().Be(_user.PasswordHash);
        }

        [Fact]
        async public Task GetUserById_RandomGuid_Return404()
        {
            var token = await AuthHelper.GetToken(_client, _admin.Email, "pwd");

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/users/{Guid.NewGuid()}");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(httpRequest);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        async public Task GetUserById_WithSimpleUserToken_Return403()
        {
            var token = await AuthHelper.GetToken(_client, _user.Email, "pwd");

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/users/{_admin.Id}");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(httpRequest);

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        async public Task GetUserById_WithoutToken_Return401()
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/users/{_user.Id}");

            var response = await _client.SendAsync(httpRequest);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        async public Task DeleteMe_ValidData_Return204()
        {
            var token = await AuthHelper.GetToken(_client, _user.Email, "pwd");

            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/users/me");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.SendAsync(httpRequest);

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var deletedUser = await _factory.UserRepositoryMock.GetByIdAsync(_user.Id);
            deletedUser.Should().BeNull();

        }

        [Fact]
        async public Task DeleteMe_WithoutToken_Return401()
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/users/me");
            
            var response = await _client.SendAsync(httpRequest);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        async public Task DeleteMe_WhenAlreadyDeleted_Return401()
        {
            var token = await AuthHelper.GetToken(_client, _user.Email, "pwd");
            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/users/me");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            await _client.SendAsync(httpRequest);

            var nextRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/users/me");
            nextRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.SendAsync(nextRequest);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        
    }
}
