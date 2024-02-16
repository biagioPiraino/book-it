namespace UserLibrary.Services.UserService;

public interface IUserService
{
    Task<Documents.User?> GetUserAsync(string userId, CancellationToken cancellationToken = default);
    
    Task<Documents.User> CreateEmptyUserAsync(string userId, string createdBy, CancellationToken cancellationToken = default);
    
    Task<Documents.User> UpdateUserAsync(string userId, Documents.User updatedUser, CancellationToken cancellationToken = default);
}