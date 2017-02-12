using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.SwaggerGen.Annotations;
using QuizManager;
using ResponseData;

namespace Question
{
    [RouteAttribute("api/quiz/questions")]
     public  class QuestionController : Controller
     {
         private IQuizManager _quizManager;
         public QuestionController(
            IQuizManager quizManager)
         {
             _quizManager = quizManager;
         }

         [HttpGet]

         [SwaggerResponse(HttpStatusCode.BadRequest)]
         public async Task<IActionResult>  GetAll()
         {
             try
             {
                var response = await _quizManager.GetAllQuestions();
                return new OkObjectResult(response);
             }
             catch(Exception ex)
             {
                 Console.WriteLine($"[Error ]{ex}");
                 return BadRequest();
             }
         } 

        [HttpGet("{id}")]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get(string id)
        {
            var response = await _quizManager.GetQuestionById(id);
            if(response != null)
                return new OkObjectResult(response);

            return NotFound();
        }  

        [HttpGet("{topicId}")]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetQuestions(string topicId)
        {
            if(string.IsNullOrEmpty(topicId))
                return BadRequest();

            var response = await _quizManager.GetQuestionByTopicId(topicId);
            if(response != null)
                return new OkObjectResult(response);

            return NotFound();
        }

        [HttpPost("{topicId}")]
        [SwaggerResponse(HttpStatusCode.Created)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Add(string topicId, 
                    [FromBodyAttribute]QuestionsIgnoreTopicIdAndQuestionId question)
        {
            try
            { 
                var topic = await _quizManager.GetTopicById(topicId);
                if( topic == null )
                {
                    return BadRequest("Topic not found");
                }

                await _quizManager.AddQuestion(new ResponseData.Question {
                    TopicId = topicId,
                    Description = question.Description,
                    Notes = question.Notes
                });
            }
            catch(Exception ex)
            {
                Console.WriteLine($"[Error ] {ex}");
                return BadRequest();
            }
            
            return new StatusCodeResult((int)HttpStatusCode.Created);
        }

        [HttpDelete("{id}")]
        public Task<IActionResult> Delete(string id)
        {
            throw new NotImplementedException();
        }

        [HttpPut("{topicId}/question")]
        public Task<IActionResult> Update(string topicId, 
            [FromBodyAttribute] QuestionsIgnoreTopicIdAndQuestionId question )
        {
            throw new NotImplementedException();
        }

        [HttpPut("{id}")]
        public Task<IActionResult> UpdateQuestion(string id, 
                [FromBodyAttribute] QuestionIgnoreId question)
        {
            throw new NotImplementedException();
        }
     }
 }