using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TopicDataContract
{
    public class Topic 
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id {get; set;}
        public string Description {get; set;}
        public string Notes {get; set;}
    }
}