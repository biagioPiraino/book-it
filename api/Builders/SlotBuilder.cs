using BookingSystem.Api.Models.DesksModels;
using DeskLibrary.Documents;

namespace BookingSystem.Api.Builders;

public static class SlotBuilder
{
    public static SlotData BuildSlotData(Slot slot)
    {
        return new SlotData
        {
            Id = slot.SlotId,
            IsAvailable = slot.IsAvailable,
            IsBooked = slot.IsBooked,
            UserId = slot.UserId,
            Day = slot.Day,
            StartingTime = slot.StartingTime,
            EndingTime = slot.EndingTime
        };
    }
}