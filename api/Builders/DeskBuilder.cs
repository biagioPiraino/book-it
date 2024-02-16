using BookingSystem.Api.Models.DesksModels;
using BookingSystem.Api.Services.Concretes;
using LibDesk = DeskLibrary.Documents.Desk;

namespace BookingSystem.Api.Builders;

public static class DeskBuilder
{
    public static Desks BuildDesks(IEnumerable<LibDesk> desks)
    {
        return new Desks
        {
            DataCount = desks.Count(),
            Data = desks.Select(BuildDeskData)
        };
    }

    public static Desk BuildDesk(LibDesk desk)
    {
        return new Desk
        {
            Data = BuildDeskData(desk)
        };
    }

    public static LibDesk BuildLibDesk(string ownerId, DeskData desk)
    {
        return new LibDesk
        {
            DeskId = desk.DeskId!,
            OwnerId = ownerId,
            Building = desk.Building!,
            Floor = desk.Floor,
            Unit = desk.Unit!,
            Image = desk.Image,
            Address = AddressBuilder.BuildLibAddress(desk.Address),
            DeskType = desk.DeskType,
            AvailableScreens = desk.AvailableScreens,
            CreatedBy = desk.CreatedBy!,
            ModifiedAt = desk.ModifiedAt,
            ModifiedBy = desk.ModifiedBy,
            Latitude = desk.Latitude,
            Longitude = desk.Longitude
        };
    }

    private static DeskData BuildDeskData(LibDesk desk)
    {
        return new DeskData
        {
            DeskId = desk.DeskId,
            Building = desk.Building,
            Floor = desk.Floor,
            Unit = desk.Unit,
            Image = desk.Image,
            Address = AddressBuilder.BuildAddressData(desk.Address),
            DeskType = desk.DeskType,
            AvailableScreens = desk.AvailableScreens,
            CreatedBy = desk.CreatedBy,
            ModifiedAt = desk.ModifiedAt,
            ModifiedBy = desk.ModifiedBy,
            SlotCount = desk.Slots.Count(),
            Slots = desk.Slots.Select(SlotBuilder.BuildSlotData).OrderBy(x => x.Day),
            Latitude = desk.Latitude,
            Longitude = desk.Longitude
        };
    }
}