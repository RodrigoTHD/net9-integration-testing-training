using AuthAPI.Models;
using System.Text;
using System.Text.Json;

namespace AuthAPI.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly JsonSerializerOptions options = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public AuthenticationService(IHttpClientFactory httpClientFactory)
    {
        this.httpClientFactory = httpClientFactory;
    }

    public async Task<(UserInfo?, HttpResponseMessage)> LoginUser(LoginInfo loginInfo)
    {
        if (string.IsNullOrWhiteSpace(loginInfo.Username) || string.IsNullOrWhiteSpace(loginInfo.Password))
        {
            throw new ArgumentNullException("User credentials connot be empty.");
        }

        var httpClient = httpClientFactory.CreateClient("dummyjson_client");

        var jsonContent = new StringContent(JsonSerializer.Serialize(loginInfo, options), Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync("user/login", jsonContent);

        if (response.IsSuccessStatusCode)
        {
            string json = await response.Content.ReadAsStringAsync();
            return (JsonSerializer.Deserialize<UserInfo>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }), response);
        }

        return (null, response);
    }
}
