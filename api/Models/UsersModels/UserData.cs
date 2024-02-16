using BookingSystem.Api.Models.AddressesModels;
using UserLibrary.Enums;

namespace BookingSystem.Api.Models.UsersModels;

public class UserData
{
    public string? Id { get; set; }
    
    public string? Title { get; set; }
    
    public string? GivenName { get; set; }
    
    public string? FamilyName { get; set; }
    
    public string? Nickname { get; set; }
    
    public string? EmailAddress { get; set; }
    
    public bool IsFederatedUser { get; set; }
    
    public Division Division { get; set; }
    
    public AddressData Address { get; set; } = new();
}