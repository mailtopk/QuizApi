using MongoDB.Driver;
using Moq;

namespace MongoHelperTest
{
    public static class MongoHelper<T>
    {
        private static Mock<IMongoDatabase> _mockMongoDatabase = null;
        private static Mock<IAsyncCursor<T>> _mockMongoCursor = null;

        private static Mock<MongoDB.Driver.IMongoCollection<T>> _mockMongoCollection = null;

        public static Mock<IMongoDatabase> GetMockMongoDBInstance()
        {   
            if(_mockMongoDatabase == null)
                _mockMongoDatabase = new Mock<IMongoDatabase>();

            return _mockMongoDatabase;
        }
        public static Mock<IAsyncCursor<T>>  GetMongoCursor()
        {
            if( _mockMongoCursor == null)
                _mockMongoCursor =  new Mock<IAsyncCursor<T>>();

            return _mockMongoCursor;
        }
        public static Mock<MongoDB.Driver.IMongoCollection<T>> GetMockMongoCollection()
        {
            if(_mockMongoCollection == null)
                _mockMongoCollection = new Mock<MongoDB.Driver.IMongoCollection<T>>();

            return _mockMongoCollection;
        }
    }
}