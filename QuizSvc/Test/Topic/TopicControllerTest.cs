using System.Collections.Generic;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using QuizDataAccess;
using System.Net;
using System.Threading.Tasks;
using QuizCaching;
using System;

namespace QuizSvcTest
{
    public class TopicTests
    {
        private Mock<IQuizDataAccess<DataEntity.Topic>> _mockDataAccess;
        private TopicController.TopicController _topicControllerMock;
        private Mock<IQuizCache<DataEntity.Topic>> _mockTopicCacheMock;
        public TopicTests()
        {
             _mockDataAccess = new Mock<IQuizDataAccess<DataEntity.Topic>>();
             _mockTopicCacheMock = new Mock<IQuizCache<DataEntity.Topic>>();

             _topicControllerMock = new TopicController.TopicController(
                 new TopicRepositoryLib.TopicRepository(_mockDataAccess.Object, _mockTopicCacheMock.Object));
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

            _mockTopicCacheMock.Setup( p => p.GetValueFromKeyAsync(
                            It.IsAny<string>(),  
                            It.IsAny<Func<string, Task<DataEntity.Topic>>>()))
                                .ReturnsAsync(mockResults);

            _mockDataAccess.Setup(
                p => p.GetAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(mockResults)
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

            _mockDataAccess.Setup(p => p.GetAllAsync())
                                .ReturnsAsync(mockResults)
                                .Verifiable();

            var result =  await _topicControllerMock.GetAll();
            Assert.IsType<ObjectResult>(result);
            var list = Assert.IsAssignableFrom<List<ResponseData.Topic>>(((ObjectResult)result).Value);
            Assert.Equal(mockResults.Count, list.Count);
        }

        [Fact]
        public async void CanAddTopic()
        {
            var mockResults = new DataEntity.Topic();
            _mockDataAccess.Setup( p => p.AddAsync(mockResults) )
                        .ReturnsAsync(string.Empty)
                        .Verifiable();
            var result = await _topicControllerMock.AddTopic(new ResponseData.TopicForAddtion());
            var actualResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)HttpStatusCode.Created, actualResult.StatusCode);
        }
    }
}