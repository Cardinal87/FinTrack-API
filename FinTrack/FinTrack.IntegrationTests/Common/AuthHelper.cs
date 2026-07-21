using FinTrack.API.DTO;
using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace FinTrack.IntegrationTests.Common
{
    public static class AuthHelper
    {
        async public static Task<string>    GetToken(HttpClient client,string login, string password)
        {
            var loginRequest = new LoginRequest
            {
                Login = login,
                Password = password
            };
            var response = await client.PostAsJsonAsync("/api/auth/token", loginRequest);
            var data = await response.Content.ReadFromJsonAsync<JsonNode>();
            return (string?)data?["access_token"] ?? "";
        }
    }
}
