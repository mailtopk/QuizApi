using System.Collections.Generic;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using QuizDataAccess;
using System.Net;
using System.Threading.Tasks;
using QuizCaching;
using System;
using Question;
using QuizRepository;

namespace QuizSvcTest
{
    public class QuestionControllerTests
    {
         private Mock<IQuizDataAccess<DataEntity.Question>> _mockDataAccess;
        private QuestionController _questionControllerMock;
        private Mock<IQuizCache<DataEntity.Question>> _mockQuestionsCacheMock;
        public QuestionControllerTests()
        {
             _mockDataAccess = new Mock<IQuizDataAccess<DataEntity.Question>>();
             _mockQuestionsCacheMock = new Mock<IQuizCache<DataEntity.Question>>();

             _questionControllerMock = new QuestionController(
                 new QuestionRepository(_mockDataAccess.Object, _mockQuestionsCacheMock.Object));
        }

         [Fact]
        public async void CanAddQuestion()
        {
            var mockResults = new DataEntity.Question();
            _mockDataAccess.Setup( p => p.AddAsync(mockResults) )
                        .ReturnsAsync(string.Empty)
                        .Verifiable();
            var result = await _questionControllerMock.Add(new ResponseData.QuestionsForAddtion());
            var actualResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)HttpStatusCode.Created, actualResult.StatusCode);
        }
    }
}