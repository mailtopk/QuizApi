using Xunit;
using System.Threading.Tasks;
using Moq;
using Microsoft.AspNetCore.Mvc;
using QuizDataAccess;
using System.Net;
using QuizCaching;
using Answer;
using QuizRepository;
using QuizManager;
using System;
using System.Collections.Generic;

namespace QuizSvcTest
{
    public class AnswerControllerTests
    {
        private Mock<IQuizDataAccess<DataEntity.Answer>> _dataAccessMock;
        private AnswerController _answerController;
        private Mock<IQuizCache<DataEntity.Answer>> _answerCacheMock;
        private IQuizManager _quizManager;
         private Mock<ITopicRepository> _topicRepositoryMock;
        private Mock<IQuestionRepository> _questionRepositoryMock;
        private Mock<IAnswerRepository> _answerRepository;
        
        public AnswerControllerTests()
        {
            _dataAccessMock = new Mock<IQuizDataAccess<DataEntity.Answer>>();
            _answerCacheMock = new Mock<IQuizCache<DataEntity.Answer>>();

             _topicRepositoryMock = new Mock<ITopicRepository>();
             _questionRepositoryMock = new Mock<IQuestionRepository>();
             _answerRepository = new Mock<IAnswerRepository>();


            _quizManager = new QuizManager.QuizManager(
                    _topicRepositoryMock.Object, 
                    _questionRepositoryMock.Object, 
                    _answerRepository.Object);

            _answerController = new AnswerController(_quizManager);

        }

        [Fact]
        public async void CanGetAnswerFromQuestionId()
        {
            var mockResponse = new List<DataEntity.Answer>{
             new DataEntity.Answer{
                Id = "answerid",
                QuestionId = "questionId",
                Description = "answer description",
                Notes = "mynotes"
            }};

            _answerCacheMock.Setup( ac => 
                        ac.GetValueFromKeyAsync(
                            It.IsAny<string>(), 
                            It.IsAny<Func<string, Task<DataEntity.Answer>>>()))
                        .ReturnsAsync(null);

            _questionRepositoryMock.Setup( qr => qr.GetQuestionAsync(It.IsAny<string>()))
                        .ReturnsAsync(new DataEntity.Question());

            _answerRepository.Setup( r => r.GetAnswerByQuestionId(It.IsAny<string>()))
                .ReturnsAsync(mockResponse);

            var result = await _answerController.Add("mockQuestionId", 
                    new ResponseData.AnswerIgnoreIdAndQuestion());

            var statusResults = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)HttpStatusCode.Created, (int)statusResults.StatusCode);

        }
    }

}