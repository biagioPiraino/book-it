using System.Text;
using BookingSystem.Api.Models.UsersModels;
using BookingSystem.Api.Services.Interfaces;

namespace BookingSystem.Api.Services.Concretes;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseApiEndpoint;
    private readonly string _token;

    public AuthService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _baseApiEndpoint = configuration.GetSection("Auth0:ApiEndpoint").Value ?? string.Empty;
        _token = configuration.GetSection("Auth0:Token").Value ?? string.Empty;
    }

    public async Task<AuthUser?> GetAuthUserAsync(string userId)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token}");
            var response = await _httpClient.GetAsync(string.Join("", _baseApiEndpoint, $"users/{userId}"));

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AuthUser>();
        }
        catch (HttpRequestException exception)
        {
            Console.WriteLine($"An HttpRequestException has been caught: {exception}");
            return null;
        }
    }

    public async Task<AuthUser?> UpdateUserAsync(string userId, UserData userData)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token}");
            
            var endpoint = string.Join("", _baseApiEndpoint, $"users/{userId}");

            // If the user is a federated user, its identity is managed by the identity provider and not by Auth0
            // In case the user wants to change personal information it should change it directly from the
            // identity provider itself (aka Google, Facebook etc.)
            if (!userData.IsFederatedUser)
            {
                var stringContent = BuildHttpContent(userData);
                var patchResponse = await _httpClient.PatchAsync(endpoint, stringContent);
                patchResponse.EnsureSuccessStatusCode();    
            }
            
            var getResponse = await _httpClient.GetAsync(string.Join("", _baseApiEndpoint, $"users/{userId}"));
            getResponse.EnsureSuccessStatusCode();
            return await getResponse.Content.ReadFromJsonAsync<AuthUser>();
        }
        catch (HttpRequestException exception)
        {
            Console.WriteLine($"An HttpRequestException has been caught: {exception}");
            return null;
        }
    }

    private static HttpContent BuildHttpContent(UserData userData)
    {
        var contentBuilder = new StringBuilder();

        contentBuilder.Append('{');
        contentBuilder.Append($"\"email\":\"{userData.EmailAddress}\",");
        contentBuilder.Append($"\"given_name\":\"{userData.GivenName}\",");
        contentBuilder.Append($"\"family_name\":\"{userData.FamilyName}\",");
        contentBuilder.Append($"\"nickname\":\"{userData.Nickname}\"");
        contentBuilder.Append($"\"name\":\"{string.Join(" ", userData.GivenName, userData.FamilyName)}\"");
        contentBuilder.Append('}');

        var content = new StringContent(contentBuilder.ToString(), Encoding.UTF8, "application/json");
        return content;
    }
}