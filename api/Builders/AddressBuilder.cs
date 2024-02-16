using BookingSystem.Api.Models.AddressesModels;
using DeskDocuments = DeskLibrary.Documents;
using UserDocuments = UserLibrary.Documents;

namespace BookingSystem.Api.Builders;

public static class AddressBuilder
{
    public static DeskDocuments.Address BuildLibAddress(AddressData address)
    {
        return new DeskDocuments.Address
        {
            AddressLine1 = address.AddressLine1!,
            AddressLine2 = address.AddressLine2,
            Postcode = address.Postcode!,
            City = address.City!,
            Country = address.Country!
        };
    }
    
    public static AddressData BuildAddressData(DeskDocuments.Address address)
    {
        return new AddressData
        {
            AddressLine1 = address.AddressLine1,
            AddressLine2 = address.AddressLine2,
            Postcode = address.Postcode,
            City = address.City,
            Country = address.Country
        };
    }
    
    public static UserDocuments.Address BuildUserLibAddress(AddressData address)
    {
        return new UserDocuments.Address
        {
            AddressLine1 = address.AddressLine1!,
            AddressLine2 = address.AddressLine2,
            Postcode = address.Postcode!,
            City = address.City!,
            Country = address.Country!
        };
    }
    
    public static AddressData BuildAddressData(UserDocuments.Address? address)
    {
        return new AddressData
        {
            AddressLine1 = address?.AddressLine1,
            AddressLine2 = address?.AddressLine2,
            Postcode = address?.Postcode,
            City = address?.City,
            Country = address?.Country
        };
    }
}