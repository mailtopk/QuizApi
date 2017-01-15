
using System.Collections.Generic;
using System.Threading.Tasks;
using DataEntity;
using QuizDataAccess;

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

        private IQuizDataAccess<Topic> quizDataAccess;

         public TopicRepository()
         {
             quizDataAccess = new QuizDataAccess<Topic>();
         }

        public async Task<IEnumerable<Topic>> GetAllTopics()
        {
            return await quizDataAccess.GetAll();
        }

        public async Task<Topic> GetTopic(string id)
        {
            return await Task.Run( () => new Topic()).ConfigureAwait(false);
        }
        
        public async Task AddTopic(Topic topic)
        {
            if( topic != null)
                await quizDataAccess.Add(topic);
        }
    }
}
