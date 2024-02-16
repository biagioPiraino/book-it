namespace BookingSystem.Api.Models.DesksModels;

public class SlotData
{
    public string Id { get; set; } = string.Empty;
    
    public string? UserId { get; set;}
    
    public bool IsAvailable { get; set; }
    
    public bool IsBooked { get; set; }
    
    public DateTime Day { get; set; }
    
    public DateTime StartingTime { get; set; }
    
    public DateTime EndingTime { get; set; }
}