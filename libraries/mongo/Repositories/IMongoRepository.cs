using System.Linq.Expressions;
using Mongo.Library.AbstractDocuments;
using MongoDB.Driver;

namespace Mongo.Library.Repositories;

public interface IMongoRepository<TDocument> where TDocument : IDocument
{
    IQueryable<TDocument> AsQueryable();

    Task<IEnumerable<TDocument>> FilterBy(
        Expression<Func<TDocument, bool>> filterExpression, 
        CancellationToken cancellationToken = default);

    Task<IEnumerable<TDocument>> FindAll(CancellationToken cancellationToken = default);
    
    IEnumerable<TProjected> FilterBy<TProjected>(
        Expression<Func<TDocument, bool>> filterExpression,
        Expression<Func<TDocument, TProjected>> projectionExpression, 
        CancellationToken cancellationToken = default);

    TDocument FindOne(Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken = default);

    Task<TDocument> FindOneAsync(Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<TDocument>> FindManyAsync(
        Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken = default);

    TDocument FindById(string id, CancellationToken cancellationToken = default);

    Task<TDocument> FindByIdAsync(string id, CancellationToken cancellationToken = default);

    void InsertOne(TDocument document, CancellationToken cancellationToken = default);

    Task InsertOneAsync(TDocument document, CancellationToken cancellationToken = default);

    void InsertMany(ICollection<TDocument> documents, CancellationToken cancellationToken = default);

    Task InsertManyAsync(ICollection<TDocument> documents, CancellationToken cancellationToken = default);

    void ReplaceOne(TDocument document, CancellationToken cancellationToken = default);

    Task ReplaceOneAsync(TDocument document, CancellationToken cancellationToken = default);
    
    Task<UpdateResult> UpdateOneAsync(
        FilterDefinition<TDocument> filter, 
        UpdateDefinition<TDocument> projectUpdates, 
        CancellationToken cancellationToken = default);

    void DeleteOne(Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken = default);

    Task DeleteOneAsync(Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken = default);

    void DeleteById(string id, CancellationToken cancellationToken = default);

    Task DeleteByIdAsync(string id, CancellationToken cancellationToken = default);

    void DeleteMany(Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken = default);

    Task DeleteManyAsync(Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken = default);
}