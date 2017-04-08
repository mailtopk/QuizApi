using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using QuizDataAccess;
using System.Net;
using Answer;
using QuizRepository;
using QuizManager;
using System.Collections.Generic;

namespace QuizSvcTest
{
    public class AnswerControllerTests
    {
        private Mock<IQuizDataAccess<DataEntity.Answer>> _dataAccessMock;
        private AnswerController _answerController;
        private IQuizManager _quizManager;
         private Mock<ITopicRepository> _topicRepositoryMock;
        private Mock<IQuestionRepository> _questionRepositoryMock;
        private Mock<IAnswerRepository> _answerRepository;
        
        public AnswerControllerTests()
        {
            _dataAccessMock = new Mock<IQuizDataAccess<DataEntity.Answer>>();
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