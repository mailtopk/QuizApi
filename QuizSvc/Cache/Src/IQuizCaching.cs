using System;
using System.Threading.Tasks;

namespace QuizCaching
{
    public interface IQuizCache<T>  where T : class
    {
        Task<T> GetValueFromKeyAsync(string key, Func<string, Task<T>> getFromDiffSource);
        Task SaveToCacheAsync(string key, T value);
    }
}