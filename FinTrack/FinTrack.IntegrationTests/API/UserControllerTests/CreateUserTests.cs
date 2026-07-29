using FinTrack.API;
using FinTrack.API.DTO;
using FinTrack.IntegrationTests.Common;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace FinTrack.IntegrationTests.API.UserControllerTests
{
    public class CreateUserTests : IClassFixture<FinTrackWebApplicationFactory<Program>>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly FinTrackWebApplicationFactory<Program> _factory;
        private readonly CancellationToken ct = TestContext.Current.CancellationToken;

        public CreateUserTests(FinTrackWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _factory = factory;
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

            var response = await _client.PostAsJsonAsync("/api/users", request, ct);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var data = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>(ct);

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

            var response = await _client.PostAsJsonAsync("/api/users", request, ct);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}