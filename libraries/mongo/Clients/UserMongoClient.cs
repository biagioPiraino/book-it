using MongoDB.Driver;

namespace Mongo.Library.Clients;

public class UserMongoClient : MongoClient
{
    public UserMongoClient(string connectionString) : base(connectionString)
    {
    }

    public UserMongoClient(MongoClientSettings clientSettings) : base(clientSettings)
    {
    }
}