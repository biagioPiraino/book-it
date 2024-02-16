using Mongo.Library.AbstractDocuments;
using Mongo.Library.Clients;
using Mongo.Library.Settings;
using MongoDB.Driver;

namespace Mongo.Library.Repositories.Desk;

public class DeskRepository<TDeskDocument> : MongoRepository<TDeskDocument>, IDeskRepository<TDeskDocument> where TDeskDocument : DeskDocument, IDocument 
{
    public DeskRepository(DeskMongoClient mongoClient, DeskSettings settings) : base(mongoClient, settings)
    {
        SetSecondaryIndex();
    }
    
    public DeskRepository(IMongoClient mongoClient, IMongoDbSettings settings) : base(mongoClient, settings)
    {
        SetSecondaryIndex();
    }

    public async Task<TDeskDocument?> FindByDeskIdAsync(string ownerId, string id, CancellationToken cancellationToken = default)
    {
        var filterBuilder = Builders<TDeskDocument>.Filter;
        var filter = filterBuilder.Eq(doc => doc.DeskId, id) & filterBuilder.Eq(doc => doc.OwnerId, ownerId);
        return await Collection.Find(filter).SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<List<TDeskDocument>> FindOwnerDesksAsync(string ownerId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TDeskDocument>.Filter.Eq(doc => doc.OwnerId, ownerId);
        return await Collection.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task<UpdateResult> UpdateDeskAsync(string ownerId, string deskId,
        UpdateDefinition<TDeskDocument> updateDefinition, CancellationToken cancellationToken = default)
    {
        var filterBuilder = Builders<TDeskDocument>.Filter;
        var filter = filterBuilder.Eq(doc => doc.DeskId, deskId) & filterBuilder.Eq(doc => doc.OwnerId, ownerId);
        return await Collection.UpdateOneAsync(filter, updateDefinition, cancellationToken: cancellationToken);
    }

    public async Task DeleteByDeskIdAsync(string ownerId, string id, CancellationToken cancellationToken = default)
    {
        var filterBuilder = Builders<TDeskDocument>.Filter;
        var filter = filterBuilder.Eq(doc => doc.DeskId, id) & filterBuilder.Eq(doc => doc.OwnerId, ownerId);
        await Collection.FindOneAndDeleteAsync(filter, cancellationToken: cancellationToken);
    }
    
    private void SetSecondaryIndex(CancellationToken cancellationToken = default)
    {
        if (HasUniqueSecondaryIndex(typeof(TDeskDocument)))
        {
            Collection.Indexes.CreateOne(new CreateIndexModel<TDeskDocument>(
                Builders<TDeskDocument>.IndexKeys.Ascending("desk_id"), 
                new CreateIndexOptions {Unique = true}), cancellationToken: cancellationToken);
            return;
        }
            
        if (HasSecondaryIndex(typeof(TDeskDocument)))
        {
            Collection.Indexes.CreateOne(new CreateIndexModel<TDeskDocument>(
                Builders<TDeskDocument>.IndexKeys.Ascending("desk_id")), cancellationToken: cancellationToken);
        }
    }
}