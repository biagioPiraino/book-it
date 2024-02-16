using DeskLibrary.Documents;

namespace DeskLibrary.Services;

public partial class DeskService
{
    private IEnumerable<Slot> GetNextWorkingWeekSlots()
    {
        var nextWorkingWeekSlots = new List<Slot>();
        
        var iteratedDay = DateTime.Today;
        var addedWorkingDays = 0;
        var dayToAdd = 1;

        while (addedWorkingDays != 5)
        {
            var dateToInsert = iteratedDay.AddDays(dayToAdd);
            if (dateToInsert.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            {
                dayToAdd++;
                continue;
            }
            
            var newSlot = new Slot
            {
                SlotId = Guid.NewGuid().ToString(),
                IsAvailable = true,
                IsBooked = false,
                Day = dateToInsert,
            };

            newSlot.StartingTime = newSlot.Day.AddHours(BeginningHour);
            newSlot.EndingTime = newSlot.StartingTime.AddHours(WorkingHours);
            
            nextWorkingWeekSlots.Add(newSlot);

            dayToAdd++;
            addedWorkingDays++;
        }

        return nextWorkingWeekSlots;
    }
}