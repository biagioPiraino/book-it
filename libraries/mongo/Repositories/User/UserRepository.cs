using Mongo.Library.AbstractDocuments;
using Mongo.Library.Clients;
using Mongo.Library.Settings;
using MongoDB.Driver;

namespace Mongo.Library.Repositories.User;

public class UserRepository<TUserDocument> : MongoRepository<TUserDocument>, IUserRepository<TUserDocument> where TUserDocument : UserDocument, IDocument 
{
    public UserRepository(UserMongoClient mongoClient, UserSettings settings) : base(mongoClient, settings)
    {
        SetSecondaryIndex();
    }
    
    public UserRepository(IMongoClient mongoClient, IMongoDbSettings settings) : base(mongoClient, settings)
    {
        SetSecondaryIndex();
    }

    public async Task<TUserDocument> FindByUserIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TUserDocument>.Filter.Eq(doc => doc.UserId, id);
        return await Collection.Find(filter).SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<UpdateResult> UpdateUserAsync(string userId, UpdateDefinition<TUserDocument> updateDefinition, CancellationToken cancellationToken = default)
    {
        var filterBuilder = Builders<TUserDocument>.Filter;
        var filter = filterBuilder.Eq(doc => doc.UserId, userId);
        return await Collection.UpdateOneAsync(filter, updateDefinition, cancellationToken: cancellationToken);
    }

    public async Task<TUserDocument> ReplaceOneByUserIdAsync(TUserDocument document, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TUserDocument>.Filter.Eq(doc => doc.UserId, document.UserId);
        var result = await Collection.FindOneAndReplaceAsync(filter, document, 
            new FindOneAndReplaceOptions<TUserDocument> { IsUpsert = true }, cancellationToken: cancellationToken);
        return result;
    }

    public async Task DeleteByUserIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TUserDocument>.Filter.Eq(doc => doc.UserId, id);
        await Collection.FindOneAndDeleteAsync(filter, cancellationToken: cancellationToken);
    }
    
    private void SetSecondaryIndex(CancellationToken cancellationToken = default)
    {
        if (HasUniqueSecondaryIndex(typeof(TUserDocument)))
        {
            Collection.Indexes.CreateOne(new CreateIndexModel<TUserDocument>(
                Builders<TUserDocument>.IndexKeys.Ascending("user_id"), 
                new CreateIndexOptions {Unique = true}), cancellationToken: cancellationToken);
            return;
        }
            
        if (HasSecondaryIndex(typeof(TUserDocument)))
        {
            Collection.Indexes.CreateOne(new CreateIndexModel<TUserDocument>(
                Builders<TUserDocument>.IndexKeys.Ascending("user_id")), cancellationToken: cancellationToken);
        }
    }
}