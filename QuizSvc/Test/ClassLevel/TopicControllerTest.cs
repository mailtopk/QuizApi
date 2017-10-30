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
using MongoHelperTest;
using Microsoft.AspNetCore.JsonPatch;
using System.Linq;

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

        public TopicControllerTests()
        {
             _dataAccessMock = new Mock<IQuizDataAccess<DataEntity.Topic>>();
             _topicCacheMock = new Mock<IQuizCache<DataEntity.Topic>>();
             _topicRepository = new Mock<ITopicRepository>();
             _quizRepository = new Mock<IQuestionRepository>();
             _answerRepository = new Mock<IAnswerRepository>();
             _loggerMock = new Mock<ILogger<TopicController.TopicController>>();

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
  
            MockTopicCache(mockResults);

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
            var list = Assert.IsAssignableFrom<IReadOnlyCollection<ResponseData.Topic>>(((ObjectResult)result).Value);
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
        public async void CanPartialUpdateTopicDescription()
        {
            _dataAccessMock.Setup(dal => dal.Update<DataEntity.Topic>(
                    It.IsAny<string>(), It.IsAny<Expression<Func<DataEntity.Topic>>>()))
                .ReturnsAsync(new DataEntity.Topic())
                .Verifiable();

            MockTopicCache( new DataEntity.Topic {
                Id = "5883a3fa50f5fea2822f21cf",
                Description = "mockedDescription",
                Notes = "mockNotes"
            } );

            var topicRep = new TopicRepository(_dataAccessMock.Object, _topicCacheMock.Object);
            var topicController = new TopicController.TopicController(new QuizManager.QuizManager(topicRep, null, null), null);

            var jsonPatchRequest = new JsonPatchDocument<ResponseData.TopicIgnoreUniqId>();
            jsonPatchRequest.Replace((q) => q.Description, "mockDescriptionToUpdate");

            var result = await topicController.UpdateTopic("mockId", jsonPatchRequest);

            var objectResult = Assert.IsType<OkObjectResult>(result);
            objectResult.StatusCode.Should().Be(200, "Should return status code as modified");
            var updatedResult = Assert.IsAssignableFrom<ResponseData.Topic>(((ObjectResult)result).Value);
            updatedResult.Description.Should().Equals("mockDescriptionToUpdate");
            updatedResult.Notes.Should().Equals("mockNotes");
            updatedResult.Id.Should().Equals("5883a3fa50f5fea2822f21cf");
        }

        private void MockTopicCache( DataEntity.Topic topic )
        {
            var mockResults = new DataEntity.Topic
            {
                Id = topic.Id,
                Description = topic.Description,
                Notes = topic.Notes
            };

            _topicCacheMock.Setup(p => p.GetValueFromKeyAsync(
                            It.IsAny<string>(),
                            It.IsAny<Func<string, Task<DataEntity.Topic>>>()))
                                .ReturnsAsync(mockResults);
        }

        [Fact]
        public async void CanUpdateTopicEntity()
        {
             // Arrange
            DataEntity.Topic returnVal = new DataEntity.Topic {
                Description = "mockUpdateDescription",
                Notes = "mockUpdateNotes"
            };

            var mockMongoDBCollection = MongoHelper<DataEntity.Topic>.GetMockMongoCollection();
            mockMongoDBCollection.Setup( c =>  
                 c.FindOneAndUpdateAsync<DataEntity.Topic>(
                        It.IsAny<MongoDB.Driver.FilterDefinition<DataEntity.Topic>>(),
                        It.IsAny<MongoDB.Driver.UpdateDefinition<DataEntity.Topic>>(), 
                        It.IsAny<FindOneAndUpdateOptions<DataEntity.Topic, DataEntity.Topic>>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(returnVal);

            var mockMongoDatabase = MongoHelper<DataEntity.Topic>.GetMockMongoDBInstance();
            mockMongoDatabase.Setup( mdb => 
                    mdb.GetCollection<DataEntity.Topic>(It.IsAny<string>(), 
                                        It.IsAny<MongoCollectionSettings>())).Returns(mockMongoDBCollection.Object);
            
            var dataAccess = new QuizDataAccess.QuizDataAccess<DataEntity.Topic>(mockMongoDatabase.Object);
            var topicRep = new TopicRepository(dataAccess, _topicCacheMock.Object);
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
        public async void CanPartialUpdateTopicNotes()
        {
            // Arrange
            var expectedResult = new DataEntity.Topic {
                 Id = "5883a3fa50f5fea2822f21cf",
                Description = "mockedDescription",
                Notes = "mockNotesDescriptionForUpdate"
            } ;

            _dataAccessMock.Setup( dal => dal.Update<DataEntity.Topic>(
                    It.IsAny<string>(), It.IsAny<Expression<Func<DataEntity.Topic>>>()))
                .ReturnsAsync(expectedResult)
                .Verifiable();
            
            MockTopicCache( new DataEntity.Topic {
                 Id = "5883a3fa50f5fea2822f21cf",
                Description = "mockedDescription",
                Notes = "mockNotes"
            } );

            var topicRep = new TopicRepository(_dataAccessMock.Object, _topicCacheMock.Object);
            var topicController = new TopicController.TopicController( 
                            new QuizManager.QuizManager(topicRep, null, null), null);
            
            var jsonPatchRequest = new JsonPatchDocument<ResponseData.TopicIgnoreUniqId>();
            jsonPatchRequest.Replace( (q) => q.Notes, expectedResult.Notes);

            // Act
            var result = await topicController.UpdateTopic("mockId",jsonPatchRequest);
            
            // Assert
            var updatedResult = Assert.IsAssignableFrom<ResponseData.Topic>(((ObjectResult)(result)).Value);
            updatedResult.Notes.Should().BeEquivalentTo("mockNotesDescriptionForUpdate");
            updatedResult.Description.Should().BeEquivalentTo("mockedDescription");
            updatedResult.Id.Should().BeEquivalentTo("5883a3fa50f5fea2822f21cf");
        }

        [Fact]
        public async void UpdateCanHandleUnavilableResource()
        {
            // Arrange
            DataEntity.Topic returnVal = null;
            var mockMongoDBCollection = MongoHelper<DataEntity.Topic>.GetMockMongoCollection();
            mockMongoDBCollection.Setup( c =>  
                 c.FindOneAndUpdateAsync<DataEntity.Topic>(
                        It.IsAny<MongoDB.Driver.FilterDefinition<DataEntity.Topic>>(),
                        It.IsAny<MongoDB.Driver.UpdateDefinition<DataEntity.Topic>>(), 
                        It.IsAny<FindOneAndUpdateOptions<DataEntity.Topic, DataEntity.Topic>>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(returnVal);

            var mockMongoDatabase = MongoHelper<DataEntity.Topic>.GetMockMongoDBInstance();
            mockMongoDatabase.Setup( mdb => 
                    mdb.GetCollection<DataEntity.Topic>(It.IsAny<string>(), 
                                        It.IsAny<MongoCollectionSettings>())).Returns(mockMongoDBCollection.Object);
            
            var dataAccess = new QuizDataAccess.QuizDataAccess<DataEntity.Topic>(mockMongoDatabase.Object);
            
            MockTopicCache( new DataEntity.Topic {
                 Id = "5883a3fa50f5fea2822f21cf",
                Description = "mockedDescription",
                Notes = "mockNotes"
            } );

            var topicRep = new TopicRepository(dataAccess, _topicCacheMock.Object);
            var topicController = new TopicController.TopicController( new QuizManager.QuizManager(topicRep, null, null), null);
            var jsonPatchRequest = new JsonPatchDocument<ResponseData.TopicIgnoreUniqId>();
            
            jsonPatchRequest.Add( (q) => q.Description, "mockDescriptionToUpdate");

            // Act
            var result = await topicController.UpdateTopic("58e5db28e40cc200151a5ba4", jsonPatchRequest);
            
            // Assert
            var statusCode = Assert.IsType<StatusCodeResult>(result);
            statusCode.StatusCode.Should().Be(304, "Should return status code as modified");
        }
    }
}