using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
namespace QuizDataAccess
{
    public class QuizDataAccess<T> : IQuizDataAccess<T>
    {
        private string dbName = "QuizDB";
        private string dbUri = $"mongodb://backenddb:27017/";
        private MongoUrl quizDBUrl;
        private MongoClient quizMongoClient;
        private IMongoDatabase quizDatabase;
        private IMongoCollection<T> collectionsOfT;

        public QuizDataAccess()
        {
            quizDBUrl = MongoUrl.Create($"{dbUri}{dbName}");
            quizMongoClient = new MongoClient(quizDBUrl);
            quizDatabase = quizMongoClient.GetDatabase(dbName);
            collectionsOfT = quizDatabase.GetCollection<T>(typeof(T).Name);
        }

        public async Task<string> Add(T newDocument)
        {
            await collectionsOfT.InsertOneAsync(newDocument);
            return ""; //TODO FIX THIS
        }

        public T Get(T inputValue)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            var results = new List<T>();
            var documents = await collectionsOfT.FindAsync(Builders<T>.Filter.Empty);
            await documents.ForEachAsync(p => results.Add(p));
            return results;
        }
    }
}


    