using BookingSystem.Api.Models.DesksModels;
using BookingSystem.Api.Services.Interfaces;

namespace BookingSystem.Api.Services.Concretes;

public class GeoLocationService : IGeoLocationService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseApiEndpoint;

    public GeoLocationService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _baseApiEndpoint = configuration.GetSection("GeoLocation:ApiEndpoint").Value ?? string.Empty;
    }
    
    public async Task<GeoLocation?> RetrieveGeolocation(string? postcode)
    {
        try
        {
            var response = await _httpClient.GetAsync(string.Join("", _baseApiEndpoint, $"{postcode}"));
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<GeoLocation>();
        }
        catch (HttpRequestException exception)
        {
            Console.WriteLine($"An HttpRequestException has been caught: {exception}");
            return null;
        }
    }
}