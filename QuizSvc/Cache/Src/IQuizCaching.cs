using System;
using System.Threading.Tasks;

namespace QuizCaching
{
    public interface IQuizCache<T>  where T : class
    {
        Task<T> GetValueFromKeyAsync(string key, Func<string, Task<T>> asyncCallback);
        Task DeletOrUpdateFromCacheAsync(string key, Func<Task> asyncCallback);
        Task UpdateAsync(string key, T objectValue);
    }
}