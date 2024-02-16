namespace Mongo.Library.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class BsonCollectionAttribute : Attribute
{
    public string CollectionName { get; }

    public BsonCollectionAttribute(string collectionName)
    {
        CollectionName = collectionName;
    }
}

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class BsonIndexAttribute : Attribute
{
    public bool HasIndex { get; set; }
    
    public string IndexName { get; set; }

    public BsonIndexAttribute(bool hasIndex, string indexName)
    {
        HasIndex = hasIndex;
        IndexName = indexName;
    }
}


[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class BsonUniqueIndexAttribute : Attribute
{
    public bool HasUniqueIndex { get; set; }
    
    public string IndexName { get; set; }

    public BsonUniqueIndexAttribute(bool hasIndex, string indexName)
    {
        HasUniqueIndex = hasIndex;
        IndexName = indexName;
    }
}