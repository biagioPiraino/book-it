using MongoDB.Bson.Serialization.Attributes;

namespace UserLibrary.Documents;

[BsonIgnoreExtraElements]
public class Slot
{
    [BsonRequired]
    [BsonElement("desk_id")]
    public string DeskId { get; set; }
    
    [BsonRequired]
    [BsonElement("slot_id")]
    public string SlotId { get; set; }
}