using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using System.Threading.Tasks;
using Moq;
using Microsoft.AspNetCore.Mvc;
using QuizDataAccess;
using System.Net;
using QuizCaching;
using Question;
using QuizRepository;
using Microsoft.Extensions.Caching.Distributed;
using QuizManager;
using FluentAssertions;
using MongoHelperTest;
using MongoDB.Driver;
using System.Threading;
using Microsoft.AspNetCore.JsonPatch;

namespace QuizSvcTest
{
    public class QuestionControllerTests
    {
        private Mock<IQuizDataAccess<DataEntity.Question>> _questionDataAccessMock;
        private QuestionController _questionController;
        private Mock<IQuizCache<DataEntity.Question>> _questionsCacheMock;
        private Mock<IQuizCache<DataEntity.Topic>> _topicCacheMock;
        private Mock<IDistributedCache> _redisServerMock;
        private IQuizManager _quizManager;
        private Mock<ITopicRepository> _topicRepositoryMock;
        private Mock<IQuestionRepository> _questionRepositoryMock;
        private Mock<IAnswerRepository> _answerRepository;

        public QuestionControllerTests()
        {
             _questionDataAccessMock = new Mock<IQuizDataAccess<DataEntity.Question>>();
             _questionsCacheMock = new Mock<IQuizCache<DataEntity.Question>>();
             _topicCacheMock = new Mock<IQuizCache<DataEntity.Topic>>();
             _topicRepositoryMock = new Mock<ITopicRepository>();
             _questionRepositoryMock = new Mock<IQuestionRepository>();
             _answerRepository = new Mock<IAnswerRepository>();

             _quizManager = new QuizManager.QuizManager(
                    _topicRepositoryMock.Object, 
                    _questionRepositoryMock.Object, 
                    _answerRepository.Object);

             _questionController = new QuestionController(_quizManager);

            _redisServerMock = new Mock<IDistributedCache>();
        }

        [Fact]
        public async void CanAddQuestion()
        {
            var mockTopic = new DataEntity.Topic();

            _topicRepositoryMock.Setup( tr => tr.GetTopicAsync(It.IsAny<string>()))
                                .ReturnsAsync(mockTopic);
            
            var result = await _questionController.Add(new ResponseData.QuestionIgnoreId{ TopicId = "9765a3fa50f5fea28212ba" });
            var actualResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)HttpStatusCode.Created, actualResult.StatusCode);
        }

        [Fact]
        public async void CanGetResourceNotFoundWhenUnavilableTopicIdIsPassedWhileAddQuestion()
        {
            var mockResults = new DataEntity.Question();

            _questionDataAccessMock.Setup( p => p.AddAsync(mockResults) )
                        .ReturnsAsync(string.Empty)
                        .Verifiable();
            
            var result = await _questionController.Add( 
                new ResponseData.QuestionIgnoreId { TopicId = "mcokTopicId" } );

            var actualResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.NotFound, actualResult.StatusCode.Value);
        }

        [Fact]
        public async void GetResponseMustHaveATopic()
        {
            var mockResults = new List<DataEntity.Question>() { new DataEntity.Question {
                Id = "5883a3fa50f5fea2822f21cf",
                TopicId = "9765a3fa50f5fea28212ba"
            }};

           MockQuestionCache(mockResults);                               
           MockquestionDataAccess(mockResults);
           MockQuestionRepository(mockResults);

            var result = await _questionController.Get("5883a3fa50f5fea2822f21cf");
            
            var statusCodeResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, statusCodeResult.StatusCode );
            
            var topicID = Assert.IsAssignableFrom<ResponseData.Question>( 
                ((ObjectResult)result).Value).TopicId;

            Assert.NotNull(topicID);
        }

        [Fact]
        public async void CanGetNotFoundResponseWhenUnavailableTopicIdIsSend()
        {
            var mockEmptyData = new List<DataEntity.Question>();
            MockQuestionCache(mockEmptyData);                               
            MockquestionDataAccess(mockEmptyData);
            MockQuestionRepository(mockEmptyData);

            var result = await _questionController.Get("5883a3fa50f5fea2822f21cf");
            
            var statusCodeResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal((int)HttpStatusCode.NotFound, statusCodeResult.StatusCode );
            
        }

        [Fact]
        public async void CanUpdateQuestion()
        {
            // Arrange
            const string topicId = "5782a3fa50f5fea2822f21cf";
            const string description = "update Questions";
            const string notes = "testing question udpate";

            // Mock data entity
            var questionEntityReturnVal = new DataEntity.Question
            {
                TopicId = topicId,
                Description = description,
                Notes = notes
            };
            var topicEntityReturnVal = new DataEntity.Topic
            {
                Id = topicId,
                Description = "topic description",
                Notes = "topic notes"
            };


            // mongoDB 
            var mockMongoDBTopic = MockMongoDatabase<DataEntity.Topic>(topicEntityReturnVal, 
                                    new List<DataEntity.Topic> { new DataEntity.Topic() });

            var mockMongoDBQuestion = MockMongoDatabase<DataEntity.Question>(questionEntityReturnVal, new List<DataEntity.Question> {
                new DataEntity.Question()
            });
            

            var dataAccessTopicObject = new QuizDataAccess.QuizDataAccess<DataEntity.Topic>(mockMongoDBTopic.Object);
            var dataAccessQuestionObject = new QuizDataAccess.QuizDataAccess<DataEntity.Question>(mockMongoDBQuestion.Object);
            
            // Cache
            MockQuestionCache(new List<DataEntity.Question> { new DataEntity.Question()} );   
            MockTopicCache( new List<DataEntity.Topic> { new DataEntity.Topic() } );

            // Repository's
            var topicRepository = new TopicRepository(dataAccessTopicObject, _topicCacheMock.Object);
            var questionRepository = new QuestionRepository(dataAccessQuestionObject, _questionsCacheMock.Object);

            var manager = new QuizManager.QuizManager(topicRepository, questionRepository, null);
            var questionController = new QuestionController(manager);

            // Act
            var result = await questionController.Update("5883a3fa50f5fea2822f21cf", 
                                        new ResponseData.QuestionIgnoreId
                                        {
                                            TopicId = topicId,
                                            Description = description,
                                            Notes = notes
                                        });

            // Assert
            var statusCode = Assert.IsType<NoContentResult>(result);
            statusCode.Should().NotBeNull("Expected a valid status code");
            statusCode.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }

        private  Mock<IMongoDatabase> MockMongoDatabase<T>(T returnValFindOneAndUpdate, List<T> returnValFind)
        {
            var mockMongoDBCollection = MongoHelper<T>.GetMockMongoCollection();

            mockMongoDBCollection.Setup(c =>
                 c.FindOneAndUpdateAsync<T>(
                        It.IsAny<MongoDB.Driver.FilterDefinition<T>>(),
                        It.IsAny<MongoDB.Driver.UpdateDefinition<T>>(),
                        It.IsAny<FindOneAndUpdateOptions<T, T>>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(returnValFindOneAndUpdate);
            

            var mockCursor = new Mock<IAsyncCursor<T>>();

            mockCursor.SetupSequence(c => c.MoveNextAsync(
                        It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult(true)).Returns(Task.FromResult(false));

            mockCursor.SetupSequence(c => c.Current)
                        .Returns(returnValFind).Returns(null);

            mockMongoDBCollection.Setup( find => 
                    find.FindAsync<T>(
                            It.IsAny<MongoDB.Driver.FilterDefinition<T>>(), 
                            It.IsAny<FindOptions<T>>(), 
                            It.IsAny<CancellationToken>()))
                        .ReturnsAsync(mockCursor.Object);

            var mockMongoDatabase = MongoHelper<T>.GetMockMongoDBInstance();
            mockMongoDatabase.Setup(mdb =>
                    mdb.GetCollection<T>(It.IsAny<string>(),
                                        It.IsAny<MongoCollectionSettings>()))
                                        .Returns(mockMongoDBCollection.Object);
                                    
            return mockMongoDatabase;
        }

        [Fact]
        private  async void QuestionDescriptionCanBeUpdated()
        {
            // Arrange
            _questionRepositoryMock.Setup( qr => qr.GetQuestionAsync(It.IsAny<string>()) )
                    .ReturnsAsync ( new  DataEntity.Question 
                        { 
                            Id = "123123", 
                            Notes = "mockNotes", 
                            TopicId = "mockTopicId",
                            Description = "This should be updated"
                        });
            _questionRepositoryMock.Setup( qr => qr.UpdateAsync(It.IsAny<string>(), It.IsAny<DataEntity.Question>()))
                    .ReturnsAsync(new DataEntity.Question {
                        TopicId = "mockTopicId",
                        Description = "mockUpdate"
                    });

            _topicRepositoryMock.Setup( tr => tr.GetTopicAsync(It.IsAny<string>()))
                .ReturnsAsync( new DataEntity.Topic() );

            var jsonPatchRequest = new JsonPatchDocument<ResponseData.QuestionIgnoreId>();
            jsonPatchRequest.Replace( (q) => q.Description, "mockUpdate");
            
            // Act
            var result = await _questionController.UpdateTopic( "updateId", jsonPatchRequest );

            // Assert
             var statusCodeResult = Assert.IsType<OkObjectResult>(result);
             statusCodeResult.Should().NotBeNull("Update Result should not be null");

             var udpatedEntity = Assert.IsAssignableFrom<ResponseData.Question>( 
                ((ObjectResult)result).Value);
            
            udpatedEntity.Should().NotBeNull();
            udpatedEntity.Description.Should().BeEquivalentTo("mockUpdate");
            udpatedEntity.TopicId.Should().Be("mockTopicId");
        }
        private void MockQuestionCache(List<DataEntity.Question> dataEntityQuestion)
        {
            _questionsCacheMock.Setup( c => c.GetValueFromKeyAsync(
                                        It.IsAny<string>(), 
                                        It.IsAny<Func<string, Task<DataEntity.Question>>>()))
                                            .ReturnsAsync(dataEntityQuestion.FirstOrDefault())
                                            .Verifiable();
        }

        private void MockTopicCache(List<DataEntity.Topic> dataEntityTopic)
        {
            _topicCacheMock.Setup( c => c.GetValueFromKeyAsync(
                                        It.IsAny<string>(), 
                                        It.IsAny<Func<string, Task<DataEntity.Topic>>>()))
                                            .ReturnsAsync(dataEntityTopic.FirstOrDefault())
                                            .Verifiable();
        }

        private void MockQuestionRepository(List<DataEntity.Question> dataEntityQuestion)
        {
            _questionRepositoryMock.Setup( q => q.GetQuestionAsync(It.IsAny<string>()))
                            .ReturnsAsync(dataEntityQuestion.FirstOrDefault())
                            .Verifiable();
        }
        private void MockquestionDataAccess(List<DataEntity.Question> dataEntityQuestion)
        {
             _questionsCacheMock.Setup( c => c.GetValueFromKeyAsync(
                                        It.IsAny<string>(), 
                                        It.IsAny<Func<string, Task<DataEntity.Question>>>()))
                                            .ReturnsAsync(dataEntityQuestion.FirstOrDefault())
                                            .Verifiable();
        }
    }
}