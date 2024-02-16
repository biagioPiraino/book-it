using Mongo.Library.Repositories.User;
using MongoDB.Driver;
using UserLibrary.Documents;

namespace UserLibrary.Services.UserService;

public class UserService : IUserService
{
    private readonly IUserRepository<User> _userRepository;

    public UserService(IUserRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> GetUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _userRepository.FindByUserIdAsync(userId, cancellationToken);
    }

    public async Task<User> CreateEmptyUserAsync(string userId, string createdBy, CancellationToken cancellationToken = default)
    {
        var emptyUser = new User
        {
            UserId = userId,
            CreatedBy = createdBy
        };

        await _userRepository.InsertOneAsync(emptyUser, cancellationToken);
        return emptyUser;
    }

    public async Task<User> UpdateUserAsync(string userId, User updatedUser, CancellationToken cancellationToken = default)
    {
        var updateDefinition = Builders<User>.Update
            .Set(desk => desk.ModifiedAt, DateTime.Now)
            .Set(desk => desk.ModifiedBy, updatedUser.ModifiedBy)
            .Set(desk => desk.Division, updatedUser.Division)
            .Set(desk => desk.Contact, updatedUser.Contact);
        
        await _userRepository.UpdateUserAsync(userId, updateDefinition, cancellationToken);
        return updatedUser;
    }
}