using System.Collections.Generic;
using System.Threading.Tasks;
namespace QuizDataAccess
{
    public interface IQuizDataAccess<T>
    {
        Task<IEnumerable<T>> GetAll();
        T Get(T inputValue);
        Task<string> Add(T newDocument);
    }
}