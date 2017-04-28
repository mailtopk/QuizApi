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
        Task<IEnumerable<Question>> GetQuestionsByTopicAsync(string topicId);
        Task AddQuestionAsync(Question topic);

        Task<Question> UpdateAsync(string questionId, Question question);
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

                     var result = await _quizDataAccess.GetByIdAsync("_id", key);
                     return result.FirstOrDefault();
             });
        }

        public async Task<IEnumerable<Question>> GetQuestionsByTopicAsync(string topicId)
        {
            return await _quizDataAccess.GetByFieldNameAsync("TopicId", topicId);
        }

        public async Task<Question> UpdateAsync(string questionId, Question question)
        {
            return await _quizDataAccess.Update(questionId,  () => new Question {
                    TopicId = question.TopicId, 
                    Description = question.Description, 
                    Notes = question.Notes
                } ) ;
        }
    }
}
