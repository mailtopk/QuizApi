
using System.Collections.Generic;
using System.Threading.Tasks;
using DataEntity;
using QuizDataAccess;

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

         public TopicRepository()
         {
             quizDataAccess = new QuizDataAccess<Topic>();
         }

        public async Task<IEnumerable<Topic>> GetAllTopicsAsync()
        {
            return await quizDataAccess.GetAllAsync();
        }

        public async Task<Topic> GetTopicAsync(string id)
        {
            return await quizDataAccess.GetAsync("_id", id);
        }
        
        public async Task AddTopicAsync(Topic topic)
        {
            if( topic != null)
                await quizDataAccess.AddAsync(topic);
        }
    }
}
