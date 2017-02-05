using System.Threading.Tasks;
using System.Collections.Generic;
using DataEntity;
using System.Linq;
using QuizDataAccess;
using QuizCaching;

namespace QuizRepository
{
    public interface IAnswerRepository
    {
        Task<IEnumerable<Answer>> GetAllAnswer();
        Task<Answer> GetAnswer(string id);
        Task AddAnswer(Answer answer);
        Task<Answer> GetAnswerByQuestionId(string questiondId);
        Task Delete(string answerId);
    }
    public class AnswerRepository : IAnswerRepository
    {
        IQuizDataAccess<Answer> _quizDataAccess;
        IQuizCache<Answer>  _cache;
        public AnswerRepository(IQuizDataAccess<Answer> quizDataAccess, 
                IQuizCache<Answer>  cache)
        {
            _quizDataAccess = quizDataAccess;
            _cache = cache;
        }

        public async Task AddAnswer(Answer answer)
        {
            await _quizDataAccess.AddAsync(answer);
        }

        public async Task<IEnumerable<Answer>> GetAllAnswer()
        {
            return await _quizDataAccess.GetAllAsync();
        }

        public Task<Answer> GetAnswer(string id)
        {
            return _cache.GetValueFromKeyAsync(id, async key => {
                var result = await _quizDataAccess.GetByIdAsync("_id", key);
                return result.FirstOrDefault();
            });
        }

        public async Task<Answer> GetAnswerByQuestionId(string questiondId)
        {
            var result  = await _quizDataAccess.GetByIdAsync("questiondId", questiondId);
            return result.FirstOrDefault();
        }

        public async Task Delete(string answerId)
        {
            await _cache.DeletFromCacheAsync( answerId, async () => 
                await _quizDataAccess.Delete(answerId)
            );
        }
    }
}