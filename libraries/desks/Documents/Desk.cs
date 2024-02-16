using DeskLibrary.Enums;
using Mongo.Library.AbstractDocuments;
using Mongo.Library.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace DeskLibrary.Documents;

[BsonIgnoreExtraElements]
[BsonCollection("desks")]
public class Desk : DeskDocument
{
    // todo: probably worth using enum with description if number is limited
    [BsonRequired]
    [BsonElement("building")]
    public string Building { get; set; }
    
    [BsonRequired]
    [BsonElement("floor")]
    public int Floor { get; set; }
    
    [BsonRequired]
    [BsonElement("unit")]
    public string Unit { get; set; }  // Unit represents the desk unit (aka name, e.g. 1A)
    
    [BsonElement("image")]
    public string? Image { get; set; }
    
    [BsonRequired]
    [BsonElement("address")]
    public Address Address { get; set; }
    
    [BsonElement("desk_type")]
    public DeskType DeskType { get; set; }
    
    [BsonElement("available_screens")]
    public int AvailableScreens { get; set; }
    
    [BsonElement("latitude")]
    public double Latitude { get; set; }
    
    [BsonElement("longitude")]
    public double Longitude { get; set; }
    
    [BsonRequired]
    [BsonElement("created_by")]
    public string CreatedBy { get; set; }
    
    [BsonElement("modified_by")]
    public string? ModifiedBy { get; set; }
    
    [BsonElement("modified_at")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
    public DateTime? ModifiedAt { get; set; }
    
    [BsonElement("slots")]
    public IEnumerable<Slot> Slots { get; set; } = new List<Slot>();
}