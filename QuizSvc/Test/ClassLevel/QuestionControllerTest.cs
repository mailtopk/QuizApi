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
using Microsoft.Extensions.Caching.Redis;

namespace QuizSvcTest
{
    public class QuestionControllerTests
    {
        private Mock<IQuizDataAccess<DataEntity.Question>> _mockQuestionDataAccess;
        private Mock<ITopicRepository> _mockTopicDataAccess;
        private QuestionController _questionControllerMock;
        private Mock<IQuizCache<DataEntity.Question>> _mockQuestionsCacheMock;
        private Mock<IDistributedCache> _mockRedisServer;
        
        public QuestionControllerTests()
        {
             _mockQuestionDataAccess = new Mock<IQuizDataAccess<DataEntity.Question>>();
             _mockQuestionsCacheMock = new Mock<IQuizCache<DataEntity.Question>>();
             _mockTopicDataAccess = new Mock<ITopicRepository>();

             _questionControllerMock = new QuestionController(
                 new QuestionRepository(_mockQuestionDataAccess.Object, _mockQuestionsCacheMock.Object),
                 _mockTopicDataAccess.Object);

            _mockRedisServer = new Mock<IDistributedCache>();
        }

         [Fact]
        public async void CanAddQuestion()
        {
            var mockResults = new DataEntity.Question();
            var mockTopic = new DataEntity.Topic();

            _mockTopicDataAccess.Setup( t => t.GetTopicAsync( It.IsAny<string>() ))
                                .ReturnsAsync( mockTopic);

            _mockQuestionDataAccess.Setup( p => p.AddAsync(mockResults) )
                        .ReturnsAsync(string.Empty)
                        .Verifiable();
            
            var result = await _questionControllerMock.Add(new ResponseData.QuestionsIgnoreId());
            var actualResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)HttpStatusCode.Created, actualResult.StatusCode);
        }

        [Fact]
        public async void GetResponseMustHaveATopic()
        {
            var mockResults = new List<DataEntity.Question>() { new DataEntity.Question {
                Id = "5883a3fa50f5fea2822f21cf",
                TopicId = "9765a3fa50f5fea28212ba"
            }};

            // TODO - Mock Redis 
            // _mockRedisServer.Setup( c => c.GetStringAsync(It.IsAny<string>()))
            //                 .ReturnsAsync(string.Empty);

           _mockQuestionsCacheMock.Setup( c => c.GetValueFromKeyAsync(
                                        It.IsAny<string>(), 
                                        It.IsAny<Func<string, Task<DataEntity.Question>>>()))
                                            .ReturnsAsync(mockResults.FirstOrDefault())
                                            .Verifiable();

            _mockQuestionDataAccess.Setup( p => p.GetByIdAsync( It.IsAny<string>(), It.IsAny<string>()))
                            .ReturnsAsync( mockResults)
                            .Verifiable();

            var result = await _questionControllerMock.Get("5883a3fa50f5fea2822f21cf");
            
            var statusCodeResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, statusCodeResult.StatusCode );
            
            var topicID = Assert.IsAssignableFrom<ResponseData.Question>( 
                ((ObjectResult)result).Value).TopicId;

            Assert.NotNull(topicID);
        }
    }
}