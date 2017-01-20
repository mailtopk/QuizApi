using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using QuizDataAccess;
using System.Net;

namespace QuizSvcTest
{
    public class TopicTests
    {
        private Mock<IQuizDataAccess<DataEntity.Topic>> mockDataAccess;
        private TopicController.TopicController topicControllerMock;
        public TopicTests()
        {
             mockDataAccess = new Mock<IQuizDataAccess<DataEntity.Topic>>();
             topicControllerMock = new TopicController.TopicController(new TopicRepositoryLib.TopicRepository(mockDataAccess.Object));
        }

        [Fact]
        public  async void CanSearchTopicByTopicID()
        {
            // Arrange
            var topicId = "587e42d3419c9d0015acbd68";
            var mockResults = new DataEntity.Topic {
                Id = topicId
            };

            mockDataAccess.Setup(
                p => p.GetAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(mockResults)
                .Verifiable();

            // Act
            var results = await topicControllerMock.GetById(topicId);

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

            mockDataAccess.Setup(p => p.GetAllAsync())
                                .ReturnsAsync(mockResults)
                                .Verifiable();

            var result =  await topicControllerMock.GetAll();
            Assert.IsType<ObjectResult>(result);
            var list = Assert.IsAssignableFrom<List<ResponseData.Topic>>(((ObjectResult)result).Value);
            Assert.Equal(mockResults.Count, list.Count);
        }

        [Fact]
        public async void CanAddTopic()
        {
            var mockResults = new DataEntity.Topic();
            mockDataAccess.Setup( p => p.AddAsync(mockResults) )
                        .ReturnsAsync(string.Empty)
                        .Verifiable();
            var result = await topicControllerMock.AddTopic(new ResponseData.TopicForAddtion());
            var actualResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)HttpStatusCode.Created, actualResult.StatusCode);
        }
    }
}
