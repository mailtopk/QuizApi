using System.Collections.Generic;
using System.Threading.Tasks;
namespace QuizDataAccess
{
    public interface IQuizDataAccess<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetAsync(string field, string value);
        Task<string> AddAsync(T newDocument);
    }
}