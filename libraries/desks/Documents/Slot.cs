using MongoDB.Bson.Serialization.Attributes;

namespace DeskLibrary.Documents;

[BsonIgnoreExtraElements]
public class Slot
{
    [BsonRequired]
    [BsonElement("slot_id")]
    public string SlotId { get; set; }
    
    [BsonElement("is_available")]
    public bool IsAvailable { get; set; }
    
    [BsonElement("is_booked")]
    public bool IsBooked { get; set; }
    
    [BsonElement("user_id")]
    public string? UserId { get; set; }
    
    [BsonRequired]
    [BsonElement("date")]
    [BsonDateTimeOptions(DateOnly = true, Kind = DateTimeKind.Local)]
    public DateTime Day { get; set; }
    
    [BsonRequired]
    [BsonElement("starting_time")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
    public DateTime StartingTime { get; set; }
    
    [BsonRequired]
    [BsonElement("ending_time")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
    public DateTime EndingTime { get; set; }
}