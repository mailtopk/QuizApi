using Xunit;
using Moq;
using TopicRepositoryLib;
using Microsoft.AspNetCore.Mvc;

namespace QuizSvcTest
{
    public class TopicTests
    {
        [Fact]
        public  async void Topic_GetId_Endpoint_Retruns_Detail_Topic_Object_TestAsync()
        {
            // Arrange
            var topicRepositoryMock = new Mock<ITopicRepository>();

            topicRepositoryMock.Setup(
                p => p.GetTopic(It.IsAny<string>()))
                .ReturnsAsync(new DataEntity.Topic());

            // Act
            var topicControllerMock = new TopicController.TopicController(topicRepositoryMock.Object);
            var results = await topicControllerMock.GetById("testVal");

            // Assert
            var actualResult = Assert.IsType<ObjectResult>(results);
            Assert.IsType<Data.Topic.Topic>(actualResult.Value);
        }
    }
}
