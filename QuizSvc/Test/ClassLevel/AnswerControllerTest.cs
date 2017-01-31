using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using QuizDataAccess;
using System.Net;
using QuizCaching;
using Answer;
using QuizRepository;

namespace QuizSvcTest
{
    public class AnswerControllerTests
    {
        private Mock<IQuizDataAccess<DataEntity.Answer>> _mockDataAccess;
        private AnswerController _answerControllerMock;
        private Mock<IQuizCache<DataEntity.Answer>> _mockAnswerCacheMock;

        public AnswerControllerTests()
        {
            _mockDataAccess = new Mock<IQuizDataAccess<DataEntity.Answer>>();
            _mockAnswerCacheMock = new Mock<IQuizCache<DataEntity.Answer>>();
        }

        public void CanGetAnswerFromId()
        {
           // TODO - work on mocking mongodb
        }
    }

}