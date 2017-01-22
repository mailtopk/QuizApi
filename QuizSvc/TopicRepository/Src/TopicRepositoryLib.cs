
using System.Collections.Generic;
using System.Threading.Tasks;
using DataEntity;
using QuizDataAccess;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace TopicRepositoryLib
{
    public interface ITopicRepository
    {
        Task<IEnumerable<Topic>> GetAllTopicsAsync();
        Task<Topic> GetTopicAsync(string id);
        Task AddTopicAsync(Topic topic);
    }
    public class TopicRepository : ITopicRepository
    {
        private IQuizDataAccess<Topic> quizDataAccess;
        private IDistributedCache cacheing;
        public TopicRepository(IQuizDataAccess<Topic> dataAccess, IDistributedCache cache)
        {
            quizDataAccess = dataAccess;
            cacheing = cache;
        }

        public async Task<IEnumerable<Topic>> GetAllTopicsAsync()
        {
            return await quizDataAccess.GetAllAsync();
        }

        // TODO - move caching implementation it its own class
        public async Task<Topic> GetTopicAsync(string id)
        {
            var result = new Topic();
            var cachedResult = await cacheing.GetStringAsync(id);

            if (cachedResult == null | string.IsNullOrEmpty(cachedResult))
            {
                result = await quizDataAccess.GetAsync("_id", id);

                if (result != null && !string.IsNullOrEmpty(result.Id))
                {
                    // SerializeObject is obsolete
                    var serializeTopic = await Task.Run(() => JsonConvert.SerializeObject(new Topic
                        {
                            Id = result.Id,
                            Description = result.Description,
                            Notes = result.Notes
                        }));
                    await cacheing.SetStringAsync(result.Id, serializeTopic);
                }
            }
            else
            {
                result = await Task.Run(() => JsonConvert.DeserializeObject<Topic>(cachedResult));
            }
            return result;
        }

        public async Task AddTopicAsync(Topic topic)
        {
            if (topic != null)
            {
                var newTopicId = await quizDataAccess.AddAsync(topic);
                if (newTopicId != null)
                {
                    // SerializeObject is obsolete - TODO move out from here
                    var serializeTopic = await Task.Run(() => JsonConvert.SerializeObject(new Topic
                        {
                            Id = topic.Id,
                            Description = topic.Description,
                            Notes = topic.Notes
                        }));
                    await cacheing.SetStringAsync(topic.Id, serializeTopic);

                }
            }
        }
    }
}
