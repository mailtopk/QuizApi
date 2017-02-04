using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataEntity
{
    public class Topic 
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id {get; set;}
        public string Description {get; set;}
        public string Notes {get; set;}
    }

    public class Question
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id {get; set;}
        public string TopicId {get; set;}
        public string Description{get; set;}
        public string Notes {get; set;}
    }

    public class Answer
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set;}
        public string QuestionId {get; set;}
        public string Description {get; set;}
        public string Notes{get; set;}
    }
}