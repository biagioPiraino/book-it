using System.Linq.Expressions;
using DeskLibrary.Documents;
using Mongo.Library.Repositories.Desk;
using MongoDB.Driver;

namespace DeskLibrary.Services;

public partial class DeskService : IDeskService
{
    private readonly IDeskRepository<Desk> _deskRepository;
    private const int BeginningHour = 9;
    private const int WorkingHours = 9;
    private const double GeoLocationRange = 0.5;

    public DeskService(IDeskRepository<Desk> deskRepository)
    {
        _deskRepository = deskRepository;
    }

    public async Task<Desk?> GetDesk(string ownerId, string deskId, CancellationToken cancellationToken = default)
    {
        return await _deskRepository.FindByDeskIdAsync(ownerId, deskId, cancellationToken);
    }
    
    public async Task<IEnumerable<Desk>> GetOwnerDesks(string ownerId, CancellationToken cancellationToken = default)
    {
        return await _deskRepository.FindOwnerDesksAsync(ownerId, cancellationToken);
    }

    public async Task<IEnumerable<Desk>> GetBookedDesks(string userId, CancellationToken cancellationToken = default)
    {
        Expression<Func<Desk, bool>> filter = desk => desk.Slots.Select(x => x.UserId).Contains(userId);
        // We first retrieve the whole desks documents
        var desks = (await _deskRepository.FindManyAsync(filter, cancellationToken)).ToList();
        
        // We than filter the slots to only include the ones that are still available and that have been booked by the user
        foreach (var desk in desks)
        {
            var filteredSlots = desk.Slots.Where(slot => slot.IsAvailable && slot.UserId == userId).ToList();
            desk.Slots = filteredSlots;
        }

        return desks;
    }

    public async Task<IEnumerable<Desk>> GetDesksByGeoLocation(double latitude, double longitude, CancellationToken cancellationToken = default)
    {
        // We retrieve desks whose geolocation is in proximity of the user search; currently we are setting this 
        // threshold to be +-0.5 degree for both latitude and longitude. This can be fine tuned according to our needs
        Expression<Func<Desk, bool>> filter = desk =>
            desk.Latitude >= latitude - GeoLocationRange &&
            desk.Latitude <= latitude + GeoLocationRange &&
            desk.Longitude >= longitude - GeoLocationRange &&
            desk.Longitude <= longitude + GeoLocationRange;
            
        return await _deskRepository.FindManyAsync(filter);
    }

    public async Task<IEnumerable<Desk>> GetAllDesks(CancellationToken cancellationToken = default)
    {
        return await _deskRepository.FindAll(cancellationToken);
    }

    public async Task<Desk> CreateDesk(string ownerId, Desk desk, CancellationToken cancellationToken = default)
    {
        desk.OwnerId = ownerId;
        desk.DeskId = Guid.NewGuid().ToString();
        await _deskRepository.InsertOneAsync(desk, cancellationToken);
        await UpdateDeskSlots(ownerId, desk.DeskId, cancellationToken);
        return desk;
    }

    // todo: this is gonna be expanded depending on the update action we are performing
    // at the moment is ok to replace all the document
    public async Task<Desk> UpdateDesk(string ownerId, Desk updatedDesk, CancellationToken cancellationToken = default)
    {
        var updateDefinition = Builders<Desk>.Update
                .Set(desk => desk.ModifiedAt, DateTime.Now)
                .Set(desk => desk.ModifiedBy, updatedDesk.ModifiedBy)
                .Set(desk => desk.Building, updatedDesk.Building)
                .Set(desk => desk.Floor, updatedDesk.Floor)
                .Set(desk => desk.Unit, updatedDesk.Unit)
                .Set(desk => desk.Address, updatedDesk.Address)
                .Set(desk => desk.DeskType, updatedDesk.DeskType)
                .Set(desk => desk.AvailableScreens, updatedDesk.AvailableScreens)
                .Set(desk => desk.Latitude, updatedDesk.Latitude)
                .Set(desk => desk.Longitude, updatedDesk.Longitude);
        
        await _deskRepository.UpdateDeskAsync(ownerId, updatedDesk.DeskId, updateDefinition, cancellationToken);
        return updatedDesk;
    }

    public async Task<Desk?> UpdateDeskSlots(string ownerId, string deskId, CancellationToken cancellationToken = default)
    {
        var desk = await _deskRepository.FindByDeskIdAsync(ownerId, deskId, cancellationToken);
        if (desk == null) return desk;

        // We keep the slot for the current week, the week before, and create the next week
        var backwardThreshold = DateTime.Today.AddDays(-7);
        
        var updatedSlots = desk.Slots.Where(x => x.Day >= backwardThreshold).ToList();
        
        // Create a new collection of slots for the working week ahead
        var nextWorkingWeekSlots = GetNextWorkingWeekSlots();
        
        // Update the entity
        updatedSlots.AddRange(nextWorkingWeekSlots);
        desk.Slots = updatedSlots;
        
        var filterBuilder = Builders<Desk>.Filter;
        var filter = filterBuilder.Eq(doc => doc.DeskId, deskId) & filterBuilder.Eq(doc => doc.OwnerId, ownerId);
        var update = Builders<Desk>.Update.Set(doc => doc.Slots, updatedSlots);
        
        await _deskRepository.UpdateOneAsync(filter, update, cancellationToken);
        
        // Return the updated entity
        return desk;
    }

    public async Task<bool> BookDeskSlot(string deskId, string slotId, string userId, CancellationToken cancellationToken = default)
    {
        Expression<Func<Desk, bool>> filter = desk => desk.DeskId == deskId && desk.Slots.Select(x => x.SlotId).Contains(slotId);

        var desk = await _deskRepository.FindOneAsync(filter, cancellationToken);
        if (desk == null) return false; // The query could actually return a null value since it search for FirstOrDefault
        
        var slotToUpdate = desk.Slots.FirstOrDefault(x => x.SlotId == slotId);

        var slotIsBookable = slotToUpdate!.IsAvailable && (!slotToUpdate.IsBooked || (slotToUpdate.IsBooked && slotToUpdate.UserId == userId));

        // Not a valid request since the desk has already been either booked by another user or unavailable
        if (!slotIsBookable) return false;
        
        // We proceed to update the slot according to the case
        var updatedSlots = desk.Slots.ToList();
        updatedSlots.RemoveAll(x => x.SlotId == slotId);
        
        // A user wants to book an available desks
        if (!slotToUpdate.IsBooked)
        {
            slotToUpdate.IsBooked = true;
            slotToUpdate.UserId = userId;
            updatedSlots.Add(slotToUpdate);
        }
        // A user wants to cancel one of his/her previous booking
        else
        {
            slotToUpdate.IsBooked = false;
            slotToUpdate.UserId = null;
            updatedSlots.Add(slotToUpdate);
        }
        
        var updateDefinition = Builders<Desk>.Update.Set(d => d.Slots, updatedSlots);
        await _deskRepository.UpdateDeskAsync(desk.OwnerId, deskId, updateDefinition, cancellationToken);
        
        return true;
    }

    public async Task<bool> UpdateDeskSlotAvailability(
        string ownerId, string deskId, string slotId, string modifiedBy, bool status, 
        CancellationToken cancellationToken = default)
    {
        var desk = await _deskRepository.FindByDeskIdAsync(ownerId, deskId, cancellationToken);
        if (desk == null) return false;
        
        var slotToUpdate = desk.Slots.FirstOrDefault(x => x.SlotId == slotId);
        if (slotToUpdate == null) return false;
        
        slotToUpdate.IsAvailable = status;

        if (slotToUpdate.IsBooked)
        {
            slotToUpdate.IsBooked = false;
            slotToUpdate.UserId = null;
        }
        
        var updatedSlots = new List<Slot>();
        updatedSlots.AddRange(desk.Slots.Where(x => x.SlotId != slotId));
        updatedSlots.Add(slotToUpdate);

        var updateDefinition = Builders<Desk>.Update
            .Set(d => d.Slots, updatedSlots)
            .Set(d => d.ModifiedBy, modifiedBy)
            .Set(d => d.ModifiedAt, DateTime.Now);
        
        await _deskRepository.UpdateDeskAsync(ownerId, deskId, updateDefinition, cancellationToken);
        return true;
    }

    public async Task<bool> SlotsAlreadyUpdated(string ownerId, string deskId, CancellationToken cancellationToken = default)
    {
        var desk = await _deskRepository.FindByDeskIdAsync(ownerId, deskId, cancellationToken);
        if (desk == null) return true; // Desk does not exist anymore, so it won't be processed
        
        // todo: this logic can be considered as a simplified version that will hold in most of the normal case scenario
        // for example, if at t0 the process of updating the slot fails midway, at t1 the desk will be regarded as been
        // already updated. Consider than to develop a function/script that will process half processed desks.
        // This algorithm will 1) remove broken slot update, 2) replace them with the a correct collection
        var dateOfProcessing = DateTime.Today;
        var slotsDates = desk.Slots.Select(x => x.Day);
        
        return slotsDates.Any(x => x > dateOfProcessing);
    }

    public async Task<bool> DeleteDesk(string ownerId, string deskId, CancellationToken cancellationToken = default)
    {
        await _deskRepository.DeleteByDeskIdAsync(ownerId, deskId, cancellationToken);
        return true;
    }
}