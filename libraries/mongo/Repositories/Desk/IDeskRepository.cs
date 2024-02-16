using Mongo.Library.AbstractDocuments;
using MongoDB.Driver;

namespace Mongo.Library.Repositories.Desk;

public interface IDeskRepository<TDeskDocument> : IMongoRepository<TDeskDocument> where TDeskDocument : IDocument
{
    Task<TDeskDocument?> FindByDeskIdAsync(string ownerId, string id, CancellationToken cancellationToken = default);
    
    Task<List<TDeskDocument>> FindOwnerDesksAsync(string ownerId, CancellationToken cancellationToken = default);

    Task<UpdateResult> UpdateDeskAsync(string ownerId, string deskId, UpdateDefinition<TDeskDocument> updateDefinition, CancellationToken cancellationToken = default);

    Task DeleteByDeskIdAsync(string ownerId, string id, CancellationToken cancellationToken = default);
}