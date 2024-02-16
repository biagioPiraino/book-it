using BookingSystem.Api.Helpers;
using BookingSystem.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UserLibrary.Services.UserService;

namespace BookingSystem.Api.Attributes.EndpointsFilters;

public class CreateEmptyUser : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var userId = context.HttpContext.User.GetUserIdentifier();
        
        if (!await UserExists(userId, context))
        {
            await InsertEmptyUser(userId, context);
        }
        
        await next();
    }

    private static async Task<bool> UserExists(string userId, ActionContext context)
    {
        var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
        var user = await userService.GetUserAsync(userId);
        return user != null;
    }

    private static async Task InsertEmptyUser(string userId, ActionContext context)
    {
        var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
        var authService = context.HttpContext.RequestServices.GetRequiredService<IAuthService>();
        
        var agent = await authService.GetAuthUserAsync(userId);
        var agentName = agent?.GetAuthUserName() ?? "Unknown user";

        await userService.CreateEmptyUserAsync(userId, agentName);
    }
}