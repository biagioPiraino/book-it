namespace DeskLibrary.Services;

public interface IDeskService
{
    Task<Documents.Desk?> GetDesk(string ownerId, string deskId, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Documents.Desk>> GetOwnerDesks(string ownerId, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Documents.Desk>> GetBookedDesks(string userId, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Documents.Desk>> GetDesksByGeoLocation(double latitude, double longitude, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Documents.Desk>> GetAllDesks(CancellationToken cancellationToken = default);
    
    Task<Documents.Desk> CreateDesk(string ownerId, Documents.Desk desk, CancellationToken cancellationToken = default);
    
    Task<Documents.Desk> UpdateDesk(string ownerId, Documents.Desk updatedDesk, CancellationToken cancellationToken = default);
    
    Task<Documents.Desk?> UpdateDeskSlots(string ownerId, string deskId, CancellationToken cancellationToken = default);
    
    Task<bool> BookDeskSlot(string deskId, string slotId, string userId, CancellationToken cancellationToken = default);
    
    Task<bool> UpdateDeskSlotAvailability(string ownerId, string deskId, string slotId, string modifiedBy, bool status, CancellationToken cancellationToken = default);
    
    Task<bool> SlotsAlreadyUpdated(string ownerId, string deskId, CancellationToken cancellationToken = default);

    Task<bool> DeleteDesk(string ownerId, string deskId, CancellationToken cancellationToken = default);
}