using Mongo.Library.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace Mongo.Library.AbstractDocuments;

[BsonIndex(true, "desk_id")]
[BsonUniqueIndex(true, "desk_id")]
public class DeskDocument : Document
{
    [BsonRequired]
    [BsonElement("desk_id")]
    public string DeskId { get; set; }
    
    [BsonRequired]
    [BsonElement("owner_id")]
    public string OwnerId { get; set; }
}