using System.Collections.Generic;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using QuizDataAccess;
using System.Net;
using System.Threading.Tasks;
using System.Linq.Expressions;
using QuizCaching;
using System;
using QuizManager;
using QuizRepository;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using MongoDB.Driver;
using System.Threading;

namespace QuizSvcTest
{
    public class TopicControllerTests
    {
        private Mock<IQuizDataAccess<DataEntity.Topic>> _dataAccessMock;
        private TopicController.TopicController _topicControllerMock;
        private Mock<IQuizCache<DataEntity.Topic>> _topicCacheMock;
        private Mock<ILogger<TopicController.TopicController>> _loggerMock;
        private IQuizManager _quizManager;

        private  Mock<ITopicRepository> _topicRepository;
        private Mock<IQuestionRepository> _quizRepository;
        private Mock<IAnswerRepository> _answerRepository;

        // MongoDB
        private Mock<IMongoDatabase> _mockMongoDatabase;
        private Mock<IAsyncCursor<DataEntity.Topic>>  _mockMongoDBCursor;
        private Mock<MongoDB.Driver.IMongoCollection<DataEntity.Topic>> _mockMongoDBCollection;

        public TopicControllerTests()
        {
             _dataAccessMock = new Mock<IQuizDataAccess<DataEntity.Topic>>();
             _topicCacheMock = new Mock<IQuizCache<DataEntity.Topic>>();
             _topicRepository = new Mock<ITopicRepository>();
             _quizRepository = new Mock<IQuestionRepository>();
             _answerRepository = new Mock<IAnswerRepository>();
             _loggerMock = new Mock<ILogger<TopicController.TopicController>>();


             // Mongo DB 
             _mockMongoDatabase = new Mock<IMongoDatabase>();
             _mockMongoDBCursor = new Mock<IAsyncCursor<DataEntity.Topic>>();
             _mockMongoDBCollection = new Mock<MongoDB.Driver.IMongoCollection<DataEntity.Topic>>();

            _quizManager = new QuizManager.QuizManager(
                    _topicRepository.Object, _quizRepository.Object, _answerRepository.Object);
            

             _topicControllerMock = new TopicController.TopicController(
                 _quizManager, _loggerMock.Object);
        }

        [Fact]
        public  async void CanSearchTopicByTopicID()
        {
            // Arrange
            var topicId = "5883a3fa50f5fea2822f21cf";
            var mockResults = new DataEntity.Topic 
                {
                    Id = topicId,
                    Description = "mockedDescription",
                    Notes = "mockNotes"
                };
  
            _topicCacheMock.Setup( p => p.GetValueFromKeyAsync(
                            It.IsAny<string>(),  
                            It.IsAny<Func<string, Task<DataEntity.Topic>>>()))
                                .ReturnsAsync(mockResults);

            _topicRepository.Setup( t => t.GetTopicAsync(It.IsAny<string>()))
                            .ReturnsAsync(mockResults)
                            .Verifiable();

            _dataAccessMock.Setup(
                p => p.GetByIdAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync( new List<DataEntity.Topic> { mockResults})
                .Verifiable();

            // Act
            var results = await _topicControllerMock.GetById(topicId);

            // Assert
            var actualResult = Assert.IsType<ObjectResult>(results);
            Assert.IsType<ResponseData.Topic>(actualResult.Value);
        }

        [Fact]
        public async void CanGetListOfAllTopic()
        {
            var mockResults = new List<DataEntity.Topic>
            {
                new DataEntity.Topic(),
                new DataEntity.Topic(),
                new DataEntity.Topic()
            };

            _topicRepository.Setup( t => t.GetAllTopicsAsync())
                            .ReturnsAsync(mockResults);

            _dataAccessMock.Setup(p => p.GetAllAsync())
                                .ReturnsAsync(mockResults)
                                .Verifiable();

            var result =  await _topicControllerMock.GetAll();
            Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<List<ResponseData.Topic>>(((ObjectResult)result).Value);
            Assert.Equal(mockResults.Count, list.Count);
        }

        [Fact]
        public async void CanAddTopic()
        {
            var mockResults = new DataEntity.Topic();
            _dataAccessMock.Setup( p => p.AddAsync(mockResults) )
                        .ReturnsAsync(string.Empty)
                        .Verifiable();
            var result = await _topicControllerMock.AddTopic(new ResponseData.TopicIgnoreUniqId());
            var actualResult = Assert.IsType<StatusCodeResult>(result);
            actualResult.StatusCode.Should().Be((int)HttpStatusCode.Created, "Status code should be created");
        }

        [Fact]
        public async void CanUpdateTopicDescription()
        {
            _dataAccessMock.Setup( dal => dal.Update<DataEntity.Topic>(
                    It.IsAny<string>(), It.IsAny<Expression<Func<DataEntity.Topic>>>()))
                .ReturnsAsync(new DataEntity.Topic())
                .Verifiable();
            
            var topicRep = new TopicRepository(_dataAccessMock.Object, null);
            var topicController = new TopicController.TopicController( new QuizManager.QuizManager(topicRep, null, null), null);
            
            var result = await topicController.UpdateDescription("mockId", "mockUpdatedDescription");
            
            var statusCode = Assert.IsType<StatusCodeResult>(result);
            statusCode.StatusCode.Should().Be(204, "Should return status code as modified");
        }

        [Fact]
        public async void CanUpdateTopicEntity()
        {
             // Arrange
            DataEntity.Topic returnVal = new DataEntity.Topic {
                Description = "mockUpdateDescription",
                Notes = "mockUpdateNotes"
            };
            _mockMongoDBCollection.Setup( c =>  
                 c.FindOneAndUpdateAsync<DataEntity.Topic>(
                        It.IsAny<MongoDB.Driver.FilterDefinition<DataEntity.Topic>>(),
                        It.IsAny<MongoDB.Driver.UpdateDefinition<DataEntity.Topic>>(), 
                        It.IsAny<FindOneAndUpdateOptions<DataEntity.Topic, DataEntity.Topic>>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(returnVal);


            _mockMongoDatabase.Setup( mdb => 
                    mdb.GetCollection<DataEntity.Topic>(It.IsAny<string>(), 
                                        It.IsAny<MongoCollectionSettings>())).Returns(_mockMongoDBCollection.Object);
            
            var dataAccess = new QuizDataAccess.QuizDataAccess<DataEntity.Topic>(_mockMongoDatabase.Object);
            var topicRep = new TopicRepository(dataAccess, null);
            var topicController = new TopicController.TopicController( new QuizManager.QuizManager(topicRep, null, null), null);
            
           var  entity = new ResponseData.Topic{
                Description = "mockDescription",
                Notes = "mockNotes"
            };

            // Act
            var result = await topicController.Update("58e5db28e40cc200151a5ba4", new ResponseData.TopicIgnoreUniqId {
                Description = entity.Description,
                Notes = entity.Notes
            } );
            
            // Assert
            var statusCode = Assert.IsType<StatusCodeResult>(result);
            statusCode.StatusCode.Should().Be(204, "Should return status code as modified");
        }

        [Fact]
        public async void CanUpdateTopicNotes()
        {
            _dataAccessMock.Setup( dal => dal.Update<DataEntity.Topic>(
                    It.IsAny<string>(), It.IsAny<Expression<Func<DataEntity.Topic>>>()))
                .ReturnsAsync(new DataEntity.Topic())
                .Verifiable();
            
            var topicRep = new TopicRepository(_dataAccessMock.Object, null);
            var topicController = new TopicController.TopicController( new QuizManager.QuizManager(topicRep, null, null), null);
            
            
            var result = await topicController.UpdateDescription("mockId", "mockUpdatedDescription");
            
            var statusCode = Assert.IsType<StatusCodeResult>(result);
            statusCode.StatusCode.Should().Be(204, "Should return status code as modified");
        }

        [Fact]
        public async void UpdateCanHandleUnavilableResource()
        {
            // Arrange
            DataEntity.Topic returnVal = null;
            _mockMongoDBCollection.Setup( c =>  
                 c.FindOneAndUpdateAsync<DataEntity.Topic>(
                        It.IsAny<MongoDB.Driver.FilterDefinition<DataEntity.Topic>>(),
                        It.IsAny<MongoDB.Driver.UpdateDefinition<DataEntity.Topic>>(), 
                        It.IsAny<FindOneAndUpdateOptions<DataEntity.Topic, DataEntity.Topic>>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(returnVal);


            _mockMongoDatabase.Setup( mdb => 
                    mdb.GetCollection<DataEntity.Topic>(It.IsAny<string>(), 
                                        It.IsAny<MongoCollectionSettings>())).Returns(_mockMongoDBCollection.Object);
            
            var dataAccess = new QuizDataAccess.QuizDataAccess<DataEntity.Topic>(_mockMongoDatabase.Object);
            var topicRep = new TopicRepository(dataAccess, null);
            var topicController = new TopicController.TopicController( new QuizManager.QuizManager(topicRep, null, null), null);
            
            // Act
            var result = await topicController.UpdateDescription("58e5db28e40cc200151a5ba4", "mockUpdatedDescription");
            
            // Assert
            var statusCode = Assert.IsType<StatusCodeResult>(result);
            statusCode.StatusCode.Should().Be(304, "Should return status code as modified");
        }
    }
}