using BookingSystem.Api.Models.UsersModels;

namespace BookingSystem.Api.Services.Interfaces;

public interface IAuthService
{
    Task<AuthUser?> GetAuthUserAsync(string userId);

    Task<AuthUser?> UpdateUserAsync(string userId, UserData userData);
}