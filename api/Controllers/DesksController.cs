using BookingSystem.Api.Builders;
using BookingSystem.Api.Helpers;
using BookingSystem.Api.Models.DesksModels;
using BookingSystem.Api.Services.Interfaces;
using DeskLibrary.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Desk = DeskLibrary.Documents.Desk;

namespace BookingSystem.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DesksController : ControllerBase
{
    private readonly IDeskService _deskService;
    private readonly IAuthService _authService;
    private readonly IGeoLocationService _geoLocationService;

    public DesksController(IDeskService deskService, IAuthService authService, IGeoLocationService geoLocationService)
    {
        _deskService = deskService;
        _authService = authService;
        _geoLocationService = geoLocationService;
    }
    

    [HttpGet]
    public async Task<IActionResult> GetOwnerDesks()
    {
        var ownerId = User.GetUserIdentifier();
        var desks = await _deskService.GetOwnerDesks(ownerId);
        var deskArray = desks as Desk[] ?? desks.ToArray();
        return deskArray.Length > 0 ? Ok(DeskBuilder.BuildDesks(deskArray)) : NoContent();
    }

    [HttpGet("geolocation")]
    public async Task<IActionResult> GetDesksByGeoLocation([FromQuery] double latitude, [FromQuery] double longitude)
    {
        var desks = await _deskService.GetDesksByGeoLocation(latitude, longitude);
        var deskArray = desks as Desk[] ?? desks.ToArray();
        return deskArray.Length > 0 ? Ok(DeskBuilder.BuildDesks(deskArray)) : NoContent();
    }

    [HttpGet("{deskId}")]
    public async Task<IActionResult> GetDesk(string deskId)
    {
        var ownerId = User.GetUserIdentifier();
        var desk = await _deskService.GetDesk(ownerId, deskId);
        return desk != null ? Ok(DeskBuilder.BuildDesk(desk)) : NoContent();
    }

    [HttpGet("booked-desks/{userId}")]
    public async Task<IActionResult> GetBookedDesks(string userId)
    {
        var desks = await _deskService.GetBookedDesks(userId);
        var deskArray = desks as Desk[] ?? desks.ToArray();
        return deskArray.Length > 0 ? Ok(DeskBuilder.BuildDesks(deskArray)) : NoContent();
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateDesk(DeskData desk)
    {
        var ownerId = User.GetUserIdentifier();
        var userProfile = await _authService.GetAuthUserAsync(ownerId);

        var builtDesk = DeskBuilder.BuildLibDesk(ownerId, desk);
        builtDesk.CreatedBy = userProfile?.GetAuthUserName() ?? "Unknown user";

        var deskGeolocation = await _geoLocationService.RetrieveGeolocation(builtDesk.Address.Postcode.Replace(" ", ""));
        builtDesk.Latitude = deskGeolocation?.Result.Latitude ?? 0;
        builtDesk.Longitude = deskGeolocation?.Result.Longitude ?? 0;
        
        var createdDesk = await _deskService.CreateDesk(ownerId, builtDesk);
        var uri = HttpContext.Request.GetEncodedPathAndQuery();
        return Created(uri, DeskBuilder.BuildDesk(createdDesk));
    }
    
    [HttpPut("{deskId}")]
    public async Task<IActionResult> UpdateDesk(DeskData desk)
    {
        var ownerId = User.GetUserIdentifier();
        var userProfile = await _authService.GetAuthUserAsync(ownerId);
        
        var libraryDesk = DeskBuilder.BuildLibDesk(ownerId, desk);
        libraryDesk.ModifiedBy = userProfile?.GetAuthUserName() ?? "Unknown user";
        
        var deskGeolocation = await _geoLocationService.RetrieveGeolocation(libraryDesk.Address.Postcode.Replace(" ", ""));
        libraryDesk.Latitude = deskGeolocation?.Result.Latitude ?? 0;
        libraryDesk.Longitude = deskGeolocation?.Result.Longitude ?? 0;
        
        var updatedDesk = await _deskService.UpdateDesk(ownerId, libraryDesk);
        return Ok(DeskBuilder.BuildDesk(updatedDesk));
    }
    
    [HttpPut("availability/{slotId}/{updatedStatus:bool}")]
    public async Task<IActionResult> UpdateDeskAvailability(bool updatedStatus, string slotId, DeskData desk)
    {
        var ownerId = User.GetUserIdentifier();
        var userProfile = await _authService.GetAuthUserAsync(ownerId);
        
        var slotIsUpdated = await _deskService.UpdateDeskSlotAvailability(
            ownerId: ownerId, 
            deskId: desk.DeskId!, 
            slotId: slotId, 
            modifiedBy: userProfile?.GetAuthUserName() ?? "Unknown user", 
            status: updatedStatus);

        var updatedDesk = await _deskService.GetDesk(ownerId, desk.DeskId!);
        
        return !slotIsUpdated ? Problem() : Ok(DeskBuilder.BuildDesk(updatedDesk!));
    }
    
    [HttpPut("{deskId}/book-slot/{slotId}")]
    public async Task<IActionResult> BookSlot(string deskId, string slotId, [FromBody] string userId)
    {
        var slotIsUpdated = await _deskService.BookDeskSlot(deskId, slotId, userId);
        return slotIsUpdated ? Ok() : Problem();
    }

    [HttpDelete("{deskId}")]
    public async Task<IActionResult> DeleteDesk(string deskId)
    {
        var ownerId = User.GetUserIdentifier();
        var desk = await _deskService.GetDesk(ownerId, deskId);
        if (desk == null) return NotFound();
        return await _deskService.DeleteDesk(ownerId, deskId) ? NoContent() : Problem();
    } 
}
