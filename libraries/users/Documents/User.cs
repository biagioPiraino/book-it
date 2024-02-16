using Mongo.Library.AbstractDocuments;
using Mongo.Library.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using UserLibrary.Enums;

namespace UserLibrary.Documents;

[BsonIgnoreExtraElements]
[BsonCollection("users")]
public class User : UserDocument
{
    [BsonElement("contacts")]
    public Contact? Contact { get; set; }
    
    [BsonRequired]
    [BsonElement("created_by")]
    public string CreatedBy { get; set; }
    
    [BsonElement("modified_by")]
    public string? ModifiedBy { get; set; }
    
    [BsonElement("modified_at")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
    public DateTime? ModifiedAt { get; set; }
    
    [BsonElement("division")]
    public Division Division { get; set; }
    
    [BsonElement("booked_slots")]
    public IEnumerable<Slot> Slots { get; set; } = new List<Slot>();
}