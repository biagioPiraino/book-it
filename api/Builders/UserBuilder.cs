using BookingSystem.Api.Models.UsersModels;
using UserLibrary.Documents;
using UserLibrary.Enums;
using LibUser = UserLibrary.Documents.User;
using User = BookingSystem.Api.Models.UsersModels.User;

namespace BookingSystem.Api.Builders;

public static class UserBuilder
{
    public static User BuildUser(AuthUser? authUser, LibUser? user)
    {
        return new User
        {
            Data = BuildUserData(authUser, user)
        };
    }

    private static UserData BuildUserData(AuthUser? authUser, LibUser? user)
    {
        return new UserData
        {
            Id = user?.UserId,
            Title = user?.Contact?.Title,
            GivenName = user?.Contact?.FirstName,
            FamilyName = user?.Contact?.LastName,
            Nickname = authUser?.Nickname,
            EmailAddress = authUser?.Email,
            Division = user?.Division ?? Division.NotAssigned,
            Address = AddressBuilder.BuildAddressData(user?.Contact?.PersonalAddress),
        };
    }

    public static LibUser BuildLibUser(UserData userData)
    {
        return new LibUser
        {
            UserId = userData.Id ?? string.Empty,
            Division = userData.Division,
            Contact = new Contact
            {
                Title = userData.Title ?? string.Empty,
                FirstName = userData.GivenName ?? string.Empty,
                LastName = userData.FamilyName ?? string.Empty,
                EmailAddress = userData.EmailAddress ?? string.Empty,
                PersonalAddress = AddressBuilder.BuildUserLibAddress(userData.Address)
            }
        };
    }
}