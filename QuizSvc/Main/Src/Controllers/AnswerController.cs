using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuizRepository;
using Swashbuckle.SwaggerGen.Annotations;

namespace Answer
{
    [Route("api/quiz/answer")]
    public class AnswerController : Controller 
    {
        private IAnswerRepository _answerRepository;
        public AnswerController(IAnswerRepository answerRepository)
        {
            _answerRepository = answerRepository;
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
                var results = await _answerRepository.GetAllAnswer();
                var responseResults = results.Select( a => new ResponseData.Answer {
                    Id = a.Id,
                    QuestionId = a.QuestionId,
                    Description = a.Description,
                    Notes = a.Notes
                } );

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
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var result = await _answerRepository.GetAnswer(id);
                if(result != null)
                {
                    return new OkObjectResult(new ResponseData.Answer {
                        Id = result.Id,
                        QuestionId = result.QuestionId,
                        Description = result.Description,
                        Notes = result.Notes
                    });
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"[Error ] {ex}");
                return BadRequest();
            }

            return new NotFoundResult();
        }

        [HttpGetAttribute("{questionId}")]
        public async Task<IActionResult> GetAnswerByQuestionId(string questionId)
        {
            try
            {
                var resuls = await _answerRepository.GetAnswerByQuestionId(questionId);
                if(resuls != null)
                {
                    return new OkObjectResult( new ResponseData.Answer {
                        Id = resuls.Id,
                        QuestionId = resuls.QuestionId,
                        Description = resuls.Description,
                        Notes = resuls.Notes
                    } );
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"[Error ] : {ex}");
                return BadRequest();
            }

            return NotFound();
        }

        [HttpGet("{topicId}")]
        public Task<IActionResult> GetAnswersByTopicId(string topicId)
        {
            // Validate topic id
            // Get all questions for given topicId
            //  - Get answer for each questions 
            // return the collection

            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBodyAttribute]ResponseData.AnswerForAddtion answer)
        {
            try
            {
               await _answerRepository.AddAnswer( new DataEntity.Answer {
                   QuestionId = answer.QuestionId,
                   Description = answer.Description,
                   Notes = answer.Notes
               });

               return new StatusCodeResult((int)HttpStatusCode.Created);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"[Error ] {ex}");
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IActionResult> Delete( string id)
        {
            await _answerRepository.Delete(id);
            return NotFound();
        }
    }
}