using MongoDB.Bson.Serialization.Attributes;

namespace UserLibrary.Documents;

[BsonIgnoreExtraElements]
public class Address
{
    [BsonRequired]
    [BsonElement("address_line_1")]
    public string AddressLine1 { get; set; }
    
    [BsonElement("address_line_2")]
    public string? AddressLine2 { get; set; }
    
    [BsonRequired]
    [BsonElement("postcode")]
    public string Postcode { get; set; }
    
    [BsonRequired]
    [BsonElement("city")]
    public string City { get; set; }
    
    [BsonRequired]
    [BsonElement("country")]
    public string Country { get; set; }
}