using System.Collections.Generic;
using System.Threading.Tasks;
namespace QuizDataAccess
{
    public interface IQuizDataAccess<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetByIdAsync(string field, string value);
        Task<IEnumerable<T>> GetByFieldNameAsync(string fieldName, string searchValue );
        Task<string> AddAsync(T newDocument);
        Task Delete(string answerId);
    }
}