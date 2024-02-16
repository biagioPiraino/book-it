using System.Text.Json.Serialization;

namespace BookingSystem.Api.Models.DesksModels;

public class GeoLocation
{
    public Result Result { get; set; } = new();
}

public class Result
{
    public double Latitude { get; set; }
    
    public double Longitude { get; set; }
} 