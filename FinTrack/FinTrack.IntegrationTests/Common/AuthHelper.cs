using FinTrack.API.DTO;
using FinTrack.API.TestMocks.Messaging;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace FinTrack.IntegrationTests.Common
{
    public static class AuthHelper
    {
        public static async Task<string> GetTokenAsync(
            HttpClient client,
            MessagePublisherMock fakePublisher,
            string login,
            string password,
            Guid userId)
        {
            var loginRequest = new LoginRequest { Login = login, Password = password };
            var response = await client.PostAsJsonAsync("/api/auth/token", loginRequest);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var data = await response.Content.ReadFromJsonAsync<JsonNode>();
                return (string?)data?["access_token"] ?? throw new NullReferenceException("Access token was not found in response");
            }

            if (response.StatusCode == HttpStatusCode.Accepted)
            {
                var data = await response.Content.ReadFromJsonAsync<JsonNode>();
                var challengeToken = (string?)data?["challenge_token"] ?? throw new NullReferenceException("Challenge token was not found in response"); ;

                var code = fakePublisher.GetCode(userId);
                if (string.IsNullOrEmpty(code))
                {
                    throw new InvalidOperationException("Fake publisher did not capture the TOTP code");
                }

                var verifyRequest = new VerifyCodeRequest { Code = code };

                var verifyMessage = new HttpRequestMessage(HttpMethod.Post, "/api/auth/2fa/complete");
                verifyMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", challengeToken);
                verifyMessage.Content = JsonContent.Create(verifyRequest);

                var verifyResponse = await client.SendAsync(verifyMessage);
                verifyResponse.EnsureSuccessStatusCode();

                var verifyData = await verifyResponse.Content.ReadFromJsonAsync<JsonNode>();
                return (string?)verifyData?["access_token"] ?? throw new NullReferenceException("Access token was not found in response");
            }

            throw new Exception($"Unexpected login status code: {response.StatusCode}. Content: {await response.Content.ReadAsStringAsync()}");
        }
    }
}
