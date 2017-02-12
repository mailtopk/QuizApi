
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataEntity;
using QuizDataAccess;
using QuizCaching;
using System;

namespace QuizRepository
{
    public interface ITopicRepository
    {
        Task<IEnumerable<Topic>> GetAllTopicsAsync();
        Task<Topic> GetTopicAsync(string id);
        Task AddTopicAsync(Topic topic);
        Task Delete(string id);
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
            try
            {
                return await _quizCache.GetValueFromKeyAsync(id, async (key) => 
                    { 
                        var result = await _quizDataAccess.GetByIdAsync("_id", key);
                        return result.FirstOrDefault();
                    } );
            }
            catch(Exception)
            {
                throw;
            }
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

        public async Task Delete(string id)
        {
            try
            {
                await _quizDataAccess.Delete(id);
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}
