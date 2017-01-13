
using System.Collections.Generic;
using System.Threading.Tasks;
using TopicDataContract;
using MongoDB.Driver;

namespace TopicRepositoryLib
{
    public interface ITopicRepository 
    {
        Task<IEnumerable<Topic>> GetAllTopics();
        Task<Topic> GetTopic(string id);
        Task AddTopic(Topic topic);
    }
    public class TopicRepository : ITopicRepository
    {
        private string dbName = "QuizDB";
        private string dbUri = $"mongodb://backenddb:27017/";

        private MongoUrl quizDBUrl; 
        private MongoClient quizMongoClient;
        private IMongoDatabase quizDatabase;
        private IMongoCollection<Topic> topicCollection;
        public TopicRepository()
        {
            quizDBUrl = MongoUrl.Create($"{dbUri}{dbName}");
            quizMongoClient = new MongoClient(quizDBUrl);
            quizDatabase = quizMongoClient.GetDatabase(dbName);
            topicCollection = quizDatabase.GetCollection<Topic>(nameof(Topic));
        }

        public async Task<IEnumerable<Topic>> GetAllTopics()
        {
            return await GetTopics();
        }

        private async Task<IEnumerable<Topic>> GetTopics()
        {
            var topics = new List<Topic>();

            var documents = await topicCollection.FindAsync(Builders<Topic>.Filter.Empty);
            await documents.ForEachAsync( t => topics.Add ( new Topic {
                Id = t.Id,
                Description = t.Description,
                Notes = t.Notes 
            } ) );

            return topics;
        }

        public async Task<Topic> GetTopic(string id)
        {
            return await Task.Run( () => new Topic()).ConfigureAwait(false);
        }
        
        public async Task AddTopic(Topic topic)
        {
            if( topic != null)
                await topicCollection.InsertOneAsync(topic);
        }
    }
}
