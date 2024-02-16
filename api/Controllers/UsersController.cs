using BookingSystem.Api.Attributes.EndpointsFilters;
using BookingSystem.Api.Builders;
using BookingSystem.Api.Models.UsersModels;
using BookingSystem.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserLibrary.Services.UserService;

namespace BookingSystem.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    public UsersController(IAuthService authService, IUserService userService)
    {
        _authService = authService;
        _userService = userService;
    }
    

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUser(string userId)
    {
        var authUser = await _authService.GetAuthUserAsync(userId);
        var user = await _userService.GetUserAsync(userId);

        if (authUser == null && user == null) return NoContent();

        return Ok(UserBuilder.BuildUser(authUser, user));
    }
    
    [CreateEmptyUser]
    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUser(string userId, [FromBody] UserData userData)
    {
        var updatedAuthUser = await _authService.UpdateUserAsync(userId, userData);
        if (updatedAuthUser == null) return Problem();
        
        var libraryUser = UserBuilder.BuildLibUser(userData);
        libraryUser.ModifiedBy = updatedAuthUser.GetAuthUserName() ?? "Unknown user";
        
        var updateUser = await _userService.UpdateUserAsync(userId, libraryUser);
        return Ok(UserBuilder.BuildUser(updatedAuthUser, updateUser));
    }
    
}