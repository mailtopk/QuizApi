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

namespace QuizSvcTest
{
    public class QuestionControllerTests
    {
        private Mock<IQuizDataAccess<DataEntity.Question>> _questionDataAccessMock;
        private Mock<ITopicRepository> _topicDataAccessMock;
        private QuestionController _questionControllerMock;
        private Mock<IQuizCache<DataEntity.Question>> _questionsCacheMock;
        private Mock<IDistributedCache> _redisServerMock;
        private IQuizManager _quizManager;
        private Mock<ITopicRepository> _topicRepositoryMock;
        private Mock<IQuestionRepository> _questionRepositoryMock;
        private Mock<IAnswerRepository> _answerRepository;

        public QuestionControllerTests()
        {
             _questionDataAccessMock = new Mock<IQuizDataAccess<DataEntity.Question>>();
             _questionsCacheMock = new Mock<IQuizCache<DataEntity.Question>>();
             _topicDataAccessMock = new Mock<ITopicRepository>();
             _topicRepositoryMock = new Mock<ITopicRepository>();
             _questionRepositoryMock = new Mock<IQuestionRepository>();
             _answerRepository = new Mock<IAnswerRepository>();

             _quizManager = new QuizManager.QuizManager(
                    _topicRepositoryMock.Object, 
                    _questionRepositoryMock.Object, 
                    _answerRepository.Object);

             _questionControllerMock = new QuestionController(_quizManager);

            _redisServerMock = new Mock<IDistributedCache>();
        }

        [Fact]
        public async void CanAddQuestion()
        {
            var mockTopic = new DataEntity.Topic();

            _topicRepositoryMock.Setup( tr => tr.GetTopicAsync(It.IsAny<string>()))
                                .ReturnsAsync(mockTopic);
            
            var result = await _questionControllerMock.Add("topicId", new ResponseData.QuestionsIgnoreTopicIdAndQuestionId{ TopicId = "9765a3fa50f5fea28212ba" });
            var actualResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)HttpStatusCode.Created, actualResult.StatusCode);
        }

        [Fact]
        public async void CanGetBadResponseWhenUnavilableTopicIdIsPassedWhileAddQuestion()
        {
            var mockResults = new DataEntity.Question();

            _topicDataAccessMock.Setup( t => t.GetTopicAsync( It.IsAny<string>() ))
                                .ReturnsAsync(null);

            _questionDataAccessMock.Setup( p => p.AddAsync(mockResults) )
                        .ReturnsAsync(string.Empty)
                        .Verifiable();
            
            var result = await _questionControllerMock.Add("topicId", new ResponseData.QuestionsIgnoreTopicIdAndQuestionId());

            var actualResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, actualResult.StatusCode.Value);
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

            var result = await _questionControllerMock.Get("5883a3fa50f5fea2822f21cf");
            
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

            var result = await _questionControllerMock.Get("5883a3fa50f5fea2822f21cf");
            
            var statusCodeResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal((int)HttpStatusCode.NotFound, statusCodeResult.StatusCode );
            
        }

        private void MockQuestionCache(List<DataEntity.Question> dataEntityQuestion)
        {
            _questionsCacheMock.Setup( c => c.GetValueFromKeyAsync(
                                        It.IsAny<string>(), 
                                        It.IsAny<Func<string, Task<DataEntity.Question>>>()))
                                            .ReturnsAsync(dataEntityQuestion.FirstOrDefault())
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