using Mongo.Library.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace Mongo.Library.AbstractDocuments;

[BsonIndex(true, "user_id")]
[BsonUniqueIndex(true, "user_id")]
public class UserDocument : Document
{
    [BsonRequired]
    [BsonElement("user_id")]
    public string UserId { get; set; }
}