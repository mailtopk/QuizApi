using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Newtonsoft.Json;

namespace QuizCaching
{
    public class QuizCache<T> : IQuizCache<T> where T : class
    {
        private IDistributedCache _caching = null;
        
        public QuizCache(IDistributedCache cache)
        {
            _caching = cache;
        }

        public async Task<T> GetValueFromKeyAsync(string key, Func<string, Task<T>> asyncCallback)
        {
            var cachedResult = await _caching.GetAsync(key);
            if(cachedResult != null)
            {
                var encodedResult = Encoding.UTF8.GetString(cachedResult);
                await  _caching.RefreshAsync(key);
                return await Task.Run(() => JsonConvert.DeserializeObject<T>(encodedResult));
            }
            else
            {
                var unCachedResults = await  asyncCallback.Invoke(key);
                if(unCachedResults != null)
                {
                    var serializeTopic = await Task.Run(() => JsonConvert.SerializeObject(unCachedResults));
                    
                    await _caching.SetStringAsync(key, serializeTopic, new DistributedCacheEntryOptions {
                        SlidingExpiration = TimeSpan.FromHours(1)
                    });
                }
                return unCachedResults;
            }
        }

        public async Task UpdateAsync(string key, T objectValue)
        {
            var serializedResult = await Task.Run(() => JsonConvert.SerializeObject(objectValue));
            if(serializedResult == null)
                throw new ArgumentNullException("Can not update null value");

            var encodedResult = Encoding.UTF8.GetBytes(serializedResult);
            await _caching.SetAsync(key, encodedResult, new DistributedCacheEntryOptions {
                        SlidingExpiration = TimeSpan.FromHours(1)});
        }

        public async Task DeletOrUpdateFromCacheAsync(string key, Func<Task> asyncCallback)
        {
            await _caching.RemoveAsync(key);

            if(asyncCallback != null)
                await asyncCallback.Invoke();
        }
    }
}
