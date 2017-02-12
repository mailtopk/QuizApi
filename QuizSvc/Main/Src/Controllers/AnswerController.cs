using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.SwaggerGen.Annotations;
using QuizManager;
using ResponseData;

namespace Answer
{
    [Route("api/quiz/answers")]
    public class AnswerController : Controller 
    {
        private IQuizManager _quizManager;
        public AnswerController(IQuizManager quizManager)
        {
            _quizManager = quizManager;
        }

        /// <summary>
        /// Get all Answer.
        /// </summary>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var results = await _quizManager.GetAllAnswers();
                return new OkObjectResult(results);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"[ERROR] : {ex}");
                return BadRequest();
            }
        }

        /// <summary>
        /// Get a specific Answer
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("{id}")]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                if(string.IsNullOrEmpty(id))
                    return BadRequest();

                var result = await _quizManager.GetAnswerById(id);
                if(result != null)
                    return new OkObjectResult(result);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"[Error ] {ex}");
                return BadRequest();
            }

            return NotFound();
        }

        [HttpPost("{questionId}/answers")]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.Created)]
        public async Task<IActionResult> Add(string questionId, 
                        [FromBodyAttribute]AnswerIgnoreIdAndQuestion answer)
        {
             try
            {
                var question = _quizManager.GetQuestionById(questionId);
                if( question == null )
                    return BadRequest("Question Not found");

                await _quizManager.AddAnswer( new ResponseData.Answer {
                    QuestionId = questionId,
                    Description = answer.Description,
                    Notes = answer.Notes
                } );
                return new StatusCodeResult((int)HttpStatusCode.Created);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"[Error ] {ex}");
                return BadRequest();
            }
        }

        [HttpPut("{questionId}/answer")]
        public Task<IActionResult> Update(string questionId, 
            [FromBodyAttribute] AnswerIgnoreIdAndQuestion answer )
        {
            throw new NotImplementedException();
        }

        [HttpPut("{id}")]
        public Task<IActionResult> Update(string id, 
                [FromBodyAttribute] AnswerIgnoreId answer)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{id}")]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.NoContent)]
        public async Task<IActionResult> Delete( string id)
        {
            var answer = _quizManager.GetAnswerById(id);
            if(answer == null)
                return NotFound("Answer not found");

            await _quizManager.DeleteAnswer(id);
            return NoContent();
        }
    }
}