using BookingSystem.Api.Models.AddressesModels;
using DeskLibrary.Enums;

namespace BookingSystem.Api.Models.DesksModels;

public class DeskData
{
    public string? DeskId { get; set; }
    
    public string? Building { get; set; }
    
    public int Floor { get; set; }
    
    public string? Unit { get; set; }
    
    public string? Image { get; set; }

    public AddressData Address { get; set; } = new();
    
    public DeskType DeskType { get; set; }
    
    public int AvailableScreens { get; set; }
    
    public double Latitude { get; set; }
    
    public double Longitude { get; set; }
    
    public string? CreatedBy { get; set; }
    
    public string? ModifiedBy { get; set; }
    
    public DateTime? ModifiedAt { get; set; }
    
    public int SlotCount { get; set; }

    public IEnumerable<SlotData> Slots { get; set; } = new List<SlotData>();
}