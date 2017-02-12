using System.Collections.Generic;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using QuizDataAccess;
using System.Net;
using System.Threading.Tasks;
using QuizCaching;
using System;
using QuizManager;
using QuizRepository;
using Microsoft.Extensions.Logging;

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
            Assert.Equal((int)HttpStatusCode.Created, actualResult.StatusCode);
        }
    }
}