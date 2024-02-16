using MongoDB.Bson;

namespace Mongo.Library.AbstractDocuments;

public abstract class Document : IDocument
{
    public ObjectId Id { get; set; }

    public DateTime CreatedAt => Id.CreationTime;
}