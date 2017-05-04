
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
        Task<Topic> UpdateTopicAsync(string id, Topic topic);
        Task DeleteAsync(string id);
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

        public async Task AddTopicAsync(Topic id)
        {
            if (id != null)
            {
                var newTopicId = await _quizDataAccess.AddAsync(id);

                // TODO - fix this 
                // if (!string.IsNullOrEmpty(newTopicId))
                // {
                //     await _quizCache.SaveToCacheAsync(newTopicId, topic);
                // }
            }
        }

        public async Task<Topic> UpdateTopicAsync(string id, Topic topic)
        {
            return await _quizDataAccess.Update<Topic>( 
                id, () => new Topic { Description = topic.Description, Notes = topic.Notes});
        }

        public async Task DeleteAsync(string id)
        {
            try
            {
                await _quizCache.DeletFromCacheAsync(id, 
                    async () => await _quizDataAccess.Delete(id) );
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}
