using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mongo.Library.AbstractDocuments;

public interface IDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    ObjectId Id { get; set; }

    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
    DateTime CreatedAt { get; }
}