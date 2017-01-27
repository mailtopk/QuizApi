using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using DataEntity;
using QuizDataAccess;
using QuizCaching;

namespace QuizRepository
{
    public interface IQuestionRepository
    {
        Task<IEnumerable<Question>> GetAllQuestionsAsync();
        Task<Question> GetQuestionAsync(string questionId);
        Task<IEnumerable<Question>> GetQuestionsByTopic(string topicId);
        Task AddQuestionAsync(Question topic);
    }

    public class QuestionRepository : IQuestionRepository
    {
        private IQuizDataAccess<Question> _quizDataAccess;
        private IQuizCache<Question> _quizCache;

        public  QuestionRepository(IQuizDataAccess<Question> quizDataAccess, 
                IQuizCache<Question>  cache )
        {
            _quizDataAccess = quizDataAccess;
            _quizCache = cache;
        }

        public async Task AddQuestionAsync(Question question)
        {
            await _quizDataAccess.AddAsync(question);
        }

        public async Task<IEnumerable<Question>> GetAllQuestionsAsync()
        {
            return await _quizDataAccess.GetAllAsync();
        }

        public async Task<Question> GetQuestionAsync(string questionId)
        {
             return await _quizCache.GetValueFromKeyAsync(questionId, async (key) =>  {

                     var result = await _quizDataAccess.GetAsync("_id", key);
                     return result.FirstOrDefault();
             });
        }

        public async Task<IEnumerable<Question>> GetQuestionsByTopic(string topicId)
        {
            return await _quizDataAccess.GetAsync("TopicId", topicId);
        }
    }
}
