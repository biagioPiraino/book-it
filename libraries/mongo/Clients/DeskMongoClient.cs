using MongoDB.Driver;

namespace Mongo.Library.Clients;

public class DeskMongoClient : MongoClient
{
    public DeskMongoClient(string connectionString) : base(connectionString)
    {
    }

    public DeskMongoClient(MongoClientSettings clientSettings) : base(clientSettings)
    {
    }
}