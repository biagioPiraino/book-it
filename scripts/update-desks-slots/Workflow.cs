using DeskLibrary.Documents;
using DeskLibrary.Services;

namespace UpdateDesksSlots;

public class Workflow : IWorkflow
{
    private readonly IDeskService _deskService;

    public Workflow(IDeskService deskService)
    {
        _deskService = deskService;
    }
    
    public async Task UpdateDesksSlots()
    {
        Console.WriteLine("Retrieving desks to update...");
        var desks = await _deskService.GetAllDesks();

        var desksToArray = desks as Desk[] ?? desks.ToArray();
        var numberOfDesks = desksToArray.Length;
        
        Console.WriteLine($"Found {numberOfDesks} to process...");
        
        for (var index = 0; index < numberOfDesks; index++ )
        {
            Console.WriteLine($"Processing desk {index + 1} of {numberOfDesks}...");
            
            var ownerId = desksToArray[index].OwnerId;
            var deskId = desksToArray[index].DeskId;

            var alreadyUpdated = await _deskService.SlotsAlreadyUpdated(ownerId, deskId);
            if (alreadyUpdated)
            {
                Console.WriteLine($"Desk {deskId} slots already updated, moving to next desk...");
            }
            else
            {
                var desk = await _deskService.UpdateDeskSlots(ownerId, deskId);
            
                Console.WriteLine(desk != null 
                    ? $"Desk {deskId} processed successfully." 
                    : $"An error occurred while processing desk {deskId}.");
            }
        }
        
        Console.WriteLine("All desks have been updated.");
    }
}