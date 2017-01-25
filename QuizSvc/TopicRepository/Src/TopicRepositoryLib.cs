
using System.Collections.Generic;
using System.Threading.Tasks;
using DataEntity;
using QuizDataAccess;
using Newtonsoft.Json;
using QuizCaching;

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
        private IQuizDataAccess<Topic> _quizDataAccess;
        private IQuizCache<Topic> _quizCache;
        public TopicRepository(IQuizDataAccess<Topic> dataAccess, 
                IQuizCache<Topic> quizCache)
        {
            _quizDataAccess = dataAccess;
            _quizCache = quizCache;
        }

        public async Task<IEnumerable<Topic>> GetAllTopicsAsync()
        {
            return await _quizDataAccess.GetAllAsync(); //TODO - pageing
        }

        public async Task<Topic> GetTopicAsync(string id)
        {
            return await _quizCache.GetValueFromKeyAsync(id, async (key) => 
                { 
                    return await _quizDataAccess.GetAsync("_id", id);
                } );
        }

        public async Task AddTopicAsync(Topic topic)
        {
            if (topic != null)
            {
                var newTopicId = await _quizDataAccess.AddAsync(topic);

                // TODO - fix this 
                // if (!string.IsNullOrEmpty(newTopicId))
                // {
                //     await _quizCache.SaveToCacheAsync(newTopicId, topic);
                // }
            }
        }
    }
}
