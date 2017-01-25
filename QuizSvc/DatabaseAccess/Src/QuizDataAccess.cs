using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

namespace QuizDataAccess
{
    public class QuizDataAccess<T> : IQuizDataAccess<T> where T : class
    {
        private const string _dbName = "QuizDB";
        private const string _dbUri = "mongodb://backenddb:27017/";
        private IMongoDatabase _mongoDatabase;
        private IMongoCollection<T> _collectionOfT;

        public QuizDataAccess()
        {
            _mongoDatabase = new MongoClient(
                        MongoUrl.Create($"{_dbUri}{_dbName}"))
                            .GetDatabase(_dbName);
                            
            _collectionOfT = _mongoDatabase.GetCollection<T>(typeof(T).Name);
        }

        public async Task<string> AddAsync(T newDocument)
        {
            await _collectionOfT.InsertOneAsync(newDocument);
            return ""; //TODO FIX THIS
        }

        public async Task<T> GetAsync(string searchField, string searchValue)
        {
            if(string.IsNullOrEmpty(searchField) || string.IsNullOrEmpty(searchValue))
                return default(T);

            var filter = Builders<T>.Filter.Eq(searchField, ObjectId.Parse(searchValue));
            return await _collectionOfT.Find<T>(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var results = new List<T>();
            var documents = await _collectionOfT.FindAsync(Builders<T>.Filter.Empty);
            await documents.ForEachAsync(p => results.Add(p));
            return results;
        }
    }
}


    