namespace BookingSystem.Api.Models.DesksModels;

public class Desks
{
    public int DataCount { get; set; }

    public IEnumerable<DeskData> Data { get; set; } = new List<DeskData>();
}