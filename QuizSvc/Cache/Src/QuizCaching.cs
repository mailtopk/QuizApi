using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
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

        public async Task<T> GetValueFromKeyAsync(string key, Func<string, Task<T>> getDataFromDiffSource)
        {
            var cachedResult = await _caching.GetStringAsync(key);

            if(cachedResult != null || !string.IsNullOrWhiteSpace(cachedResult))
                return await Task.Run(() => JsonConvert.DeserializeObject<T>(cachedResult));
            else
            {
                var unCachedResults = await  getDataFromDiffSource.Invoke(key);

                var serializeTopic = await Task.Run(() => JsonConvert.SerializeObject(unCachedResults));
                await _caching.SetStringAsync(key, serializeTopic);

                return unCachedResults;
            }
        }

        public async Task SaveToCacheAsync(string key, T objectValue)
        {
              var serializedResult = await Task.Run(() => JsonConvert.SerializeObject(objectValue));
              await _caching.SetStringAsync(key, serializedResult);
        }
    }
}
