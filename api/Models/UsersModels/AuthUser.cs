using System.Text.Json.Serialization;

namespace BookingSystem.Api.Models.UsersModels;

public class AuthUser
{
    public string? Email { get; set; }
    
    public string? Name { get; set; }
    
    [JsonPropertyName("given_name")]
    public string? GivenName { get; set; }
    
    [JsonPropertyName("family_name")]
    public string? FamilyName { get; set; }
    
    public string? Nickname { get; set; }

    public string? GetAuthUserName()
    {
        if (string.IsNullOrEmpty(FamilyName) || string.IsNullOrEmpty(GivenName))
        {
            return Name;
        }
        return string.Join(" ", GivenName, FamilyName);
    }
}