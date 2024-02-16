using Mongo.Library.AbstractDocuments;
using MongoDB.Driver;

namespace Mongo.Library.Repositories.User;

public interface IUserRepository<TUserDocument> : IMongoRepository<TUserDocument> where TUserDocument : IDocument
{
    Task<TUserDocument> FindByUserIdAsync(string id, CancellationToken cancellationToken = default);

    Task<UpdateResult> UpdateUserAsync(string userId, UpdateDefinition<TUserDocument> updateDefinition, CancellationToken cancellationToken = default);
    
    Task<TUserDocument> ReplaceOneByUserIdAsync(TUserDocument document, CancellationToken cancellationToken = default);

    Task DeleteByUserIdAsync(string id, CancellationToken cancellationToken = default);
}