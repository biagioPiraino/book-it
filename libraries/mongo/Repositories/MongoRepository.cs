using System.Linq.Expressions;
using Mongo.Library.AbstractDocuments;
using Mongo.Library.Attributes;
using Mongo.Library.Settings;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mongo.Library.Repositories;

public class MongoRepository<TDocument> : IMongoRepository<TDocument> where TDocument : IDocument
{
    protected readonly IMongoCollection<TDocument> Collection;

    protected MongoRepository(IMongoClient mongoMongoClient, IMongoDbSettings settings)
    {
        var database = mongoMongoClient.GetDatabase(settings.DatabaseName);
        Collection = database.GetCollection<TDocument>(GetCollectionName(typeof(TDocument)));
    }

    public virtual IQueryable<TDocument> AsQueryable()
    {
        return Collection.AsQueryable();
    }

    public virtual Task<IEnumerable<TDocument>> FilterBy(
        Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => Collection.Find(filterExpression).ToEnumerable(cancellationToken: cancellationToken), cancellationToken);
    }

    public virtual IEnumerable<TProjected> FilterBy<TProjected>(
        Expression<Func<TDocument, bool>> filterExpression,
        Expression<Func<TDocument, TProjected>> projectionExpression, 
        CancellationToken cancellationToken = default)
    {
        return Collection.Find(filterExpression).Project(projectionExpression).ToEnumerable(cancellationToken: cancellationToken);
    }

    public virtual Task<IEnumerable<TDocument>> FindAll(CancellationToken cancellationToken = default)
    {
        var emptyFilter = Builders<TDocument>.Filter.Empty;
        return Task.Run(() => Collection.Find(emptyFilter).ToEnumerable(cancellationToken), cancellationToken);
    }

    public virtual TDocument FindOne(
        Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken = default)
    {
        return Collection.Find(filterExpression).FirstOrDefault(cancellationToken: cancellationToken);
    }

    public virtual Task<TDocument> FindOneAsync(
        Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => Collection.Find(filterExpression).FirstOrDefaultAsync(cancellationToken: cancellationToken), cancellationToken);
    }

    public Task<IEnumerable<TDocument>> FindManyAsync(Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => Collection.Find(filterExpression).ToEnumerable(cancellationToken), cancellationToken);
    }

    public virtual TDocument FindById(string id, CancellationToken cancellationToken = default)
    {
        var objectId = new ObjectId(id);
        var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);
        return Collection.Find(filter).SingleOrDefault(cancellationToken: cancellationToken);
    }

    public virtual Task<TDocument> FindByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            var objectId = new ObjectId(id);
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);
            return Collection.Find(filter).SingleOrDefaultAsync(cancellationToken: cancellationToken);
        }, cancellationToken);
    }
    
    public virtual void InsertOne(TDocument document, CancellationToken cancellationToken = default)
    {
        Collection.InsertOne(document, cancellationToken: cancellationToken);
    }

    public virtual Task InsertOneAsync(TDocument document, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => Collection.InsertOneAsync(document, cancellationToken: cancellationToken), cancellationToken);
    }

    public void InsertMany(ICollection<TDocument> documents, CancellationToken cancellationToken = default)
    {
        Collection.InsertMany(documents, cancellationToken: cancellationToken);
    }
    
    public virtual async Task InsertManyAsync(ICollection<TDocument> documents, CancellationToken cancellationToken = default)
    {
        await Collection.InsertManyAsync(documents, cancellationToken: cancellationToken);
    }

    public void ReplaceOne(TDocument document, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);
        Collection.FindOneAndReplace(filter, document, cancellationToken: cancellationToken);
    }

    public virtual async Task ReplaceOneAsync(TDocument document, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);
        await Collection.FindOneAndReplaceAsync(filter, document, cancellationToken: cancellationToken);
    }
    
    public async Task<UpdateResult> UpdateOneAsync(
        FilterDefinition<TDocument> filterDefinition,
        UpdateDefinition<TDocument> updateDefinition, CancellationToken cancellationToken = default)
    {
            
        return await Collection.UpdateOneAsync(filterDefinition, updateDefinition, cancellationToken: cancellationToken);
    }

    public void DeleteOne(Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken = default)
    {
        Collection.FindOneAndDelete(filterExpression, cancellationToken: cancellationToken);
    }

    public Task DeleteOneAsync(Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => Collection.FindOneAndDeleteAsync(filterExpression, cancellationToken: cancellationToken), cancellationToken);
    }

    public void DeleteById(string id, CancellationToken cancellationToken = default)
    {
        var objectId = new ObjectId(id);
        var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);
        Collection.FindOneAndDelete(filter, cancellationToken: cancellationToken);
    }

    public Task DeleteByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            var objectId = new ObjectId(id);
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);
            Collection.FindOneAndDeleteAsync(filter, cancellationToken: cancellationToken);
        }, cancellationToken);
    }

    public void DeleteMany(Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken = default)
    {
        Collection.DeleteMany(filterExpression, cancellationToken);
    }

    public Task DeleteManyAsync(Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => Collection.DeleteManyAsync(filterExpression, cancellationToken), cancellationToken);
    }
    
    private string GetCollectionName(Type documentType)
    {
        var customAttribute = documentType.GetCustomAttributes(typeof(BsonCollectionAttribute), true).FirstOrDefault();
        var bsonCollectionAttribute = customAttribute as BsonCollectionAttribute;
        return bsonCollectionAttribute?.CollectionName ?? string.Empty;
    }

    protected bool HasSecondaryIndex(Type documentType)
    {
        var customAttribute = documentType.GetCustomAttributes(typeof(BsonIndexAttribute), true).FirstOrDefault();
        var bsonCollectionAttribute = customAttribute as BsonIndexAttribute;
        return bsonCollectionAttribute?.HasIndex ?? false;
    }
     
    protected bool HasUniqueSecondaryIndex(Type documentType)
    {
        var customAttribute = documentType.GetCustomAttributes(typeof(BsonUniqueIndexAttribute), true).FirstOrDefault();
        var bsonCollectionAttribute = customAttribute as BsonUniqueIndexAttribute;
        return bsonCollectionAttribute?.HasUniqueIndex ?? false;
    }
}