using BookingSystem.Api.Models.DesksModels;

namespace BookingSystem.Api.Services.Interfaces;

public interface IGeoLocationService
{
    Task<GeoLocation?> RetrieveGeolocation(string postcode);
}