using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Linq.Expressions;
using System;
using QuizHelper;

namespace QuizDataAccess
{
    public class QuizDataAccess<T> : IQuizDataAccess<T> where T : class
    {
        private const string _dbName = "QuizDB";
        private const string _dbUri = "mongodb://backenddb:27017/";
        private IMongoDatabase _mongoDatabase;
        private IMongoCollection<T> _collectionOfT;

        public QuizDataAccess(IMongoDatabase mongoClient = null)
        {
            _mongoDatabase = mongoClient ?? new MongoClient(
                        MongoUrl.Create($"{_dbUri}{_dbName}"))
                            .GetDatabase(_dbName);
                            
            _collectionOfT = _mongoDatabase.GetCollection<T>(typeof(T).Name);
        }

        public async Task<string> AddAsync(T newDocument)
        {
            await _collectionOfT.InsertOneAsync(newDocument);
            return ""; //TODO FIX THIS
        }

        public async Task<IEnumerable<T>> GetByIdAsync(string searchField, string searchValue)
        {
            if(string.IsNullOrEmpty(searchField) || string.IsNullOrEmpty(searchValue))
                return default(IEnumerable<T>);

            var filter = Builders<T>.Filter.Eq(searchField, ObjectId.Parse(searchValue));

            var cursor = await _collectionOfT.FindAsync<T>(filter);

            return await Task.Run( async () => {
                
                List<T> buffer = new List<T>();
                await cursor.ForEachAsync<T>( p => buffer.Add(p));
                
                return buffer;
            } );
        }

        // TODO combine GetByIdAsync
        public async Task<IEnumerable<T>> GetByFieldNameAsync(string fieldName, string searchString )
        {
            var filter = Builders<T>.Filter.Eq(fieldName, searchString);
            var cursor = await _collectionOfT.FindAsync<T>(filter);

            return await Task.Run( async () => {
                
                List<T> buffer = new List<T>();
                await cursor.ForEachAsync<T>( p => buffer.Add(p));
                
                return buffer;
            } );
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var results = new List<T>();
            var documents = await _collectionOfT.FindAsync(Builders<T>.Filter.Empty);
            await documents.ForEachAsync(p => results.Add(p));
            return results;
        }

        public async Task<T> Update<TUpdate>(string documentId, 
                                        Expression<Func<TUpdate>> entityToUpdate)
        {
            var updateBuilders = new List<UpdateDefinition<T>>();
            var memberBindingsAndValue = Helper.ExtractBindingsAndValues(entityToUpdate);
            
            if( memberBindingsAndValue == null)
                throw new Exception("Invalid expression");
                
            foreach (var item in memberBindingsAndValue)
            {

                var binding = item.Key;
                var value = item.Value;
                if(binding == null)
                    throw new Exception("invalid binding and value");

                var set = MongoDB.Driver.Builders<T>.Update.Set(binding, value);
                updateBuilders.Add(set);
            }

            try
            {
                if(updateBuilders.Count == 0)
                    return null;

                var filterDef = Builders<T>.Filter.Eq("_id", ObjectId.Parse(documentId));
                var combineUpdateBuilder = MongoDB.Driver.Builders<T>.Update.Combine(updateBuilders);
                var options = new FindOneAndUpdateOptions<T>
                    {
                        ReturnDocument = ReturnDocument.After
                    };

                return  await _collectionOfT.FindOneAndUpdateAsync<T>(filterDef, combineUpdateBuilder, options);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }
         
        public async Task Delete(string id)
        {
            var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
            await _collectionOfT.DeleteOneAsync(filter);
        }
    }
}


    