using MongoDB.Bson.Serialization.Attributes;

namespace UserLibrary.Documents;

[BsonIgnoreExtraElements]
public class Contact
{
    [BsonRequired]
    [BsonElement("title")]
    public string Title { get; set; }
    
    [BsonRequired]
    [BsonElement("first_name")]
    public string FirstName { get; set; }
    
    [BsonRequired]
    [BsonElement("last_name")]
    public string LastName { get; set; }
    
    [BsonRequired]
    [BsonElement("email_address")]
    public string EmailAddress { get; set; }
    
    // todo: probably worth having a separation between personal and billing address
    [BsonElement("personal_address")]
    public Address? PersonalAddress { get; set; }
}