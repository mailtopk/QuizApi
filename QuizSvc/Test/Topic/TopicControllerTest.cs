using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using QuizDataAccess;

namespace QuizSvcTest
{
    public class TopicTests
    {
        private Mock<IQuizDataAccess<DataEntity.Topic>> dataAccess;

        public TopicTests()
        {
             dataAccess = new Mock<IQuizDataAccess<DataEntity.Topic>>();
        }

        [Fact]
        public  async void Topic_GetId_Endpoint_Retruns_Detail_Topic_Object_TestAsync()
        {
            // Arrange
            //var dataAccess = new Mock<IQuizDataAccess<DataEntity.Topic>>();
            var topicId = "587e42d3419c9d0015acbd68";
            var expectedResults = new DataEntity.Topic {
                Id = topicId
            };

            var topicControllerMock = new TopicController.TopicController( 
                new TopicRepositoryLib.TopicRepository(dataAccess.Object));

            dataAccess.Setup(
                p => p.GetAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(expectedResults);

            // Act
            var results = await topicControllerMock.GetById(topicId);

            // Assert
            var actualResult = Assert.IsType<ObjectResult>(results);
            Assert.IsType<Data.Topic.Topic>(actualResult.Value);
        }
    }
}
