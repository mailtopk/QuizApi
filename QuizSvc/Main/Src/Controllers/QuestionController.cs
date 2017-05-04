using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.SwaggerGen.Annotations;
using QuizManager;
using ResponseData;
using Microsoft.AspNetCore.JsonPatch;

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

         /// <summary>Get all Questions</summary>
         [HttpGet]
         [SwaggerResponse(HttpStatusCode.BadRequest)]
         public async Task<IActionResult>  GetAll()
         {
             try
             {
                var response = await _quizManager.GetAllQuestionsAsync();
                return new OkObjectResult(response);
             }
             catch(Exception ex)
             {
                 Console.WriteLine($"[Error ]{ex}");
                 return BadRequest();
             }
         } 

         /// <summary>Get Question by id</summary>
         /// <param name="topicId">Topic Id</param>
        [HttpGet("{topicId}")]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get(string topicId)
        {
            var response = await _quizManager.GetQuestionByIdAsync(topicId);
            if(response != null)
                return new OkObjectResult(response);

            return NotFound();
        }  

        /// <summary>Get Question for a given topic</summary>
        /// <param name="topicId">Topic Id</param>
        [HttpGet("{topicId}")]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetQuestions(string topicId)
        {
            if(string.IsNullOrEmpty(topicId))
                return BadRequest();

            var response = await _quizManager.GetQuestionByTopicIdAsync(topicId);
            if(response != null)
                return new OkObjectResult(response);

            return NotFound();
        }

        /// <summary>Create new Question</summary>
        /// <param name="question">Question</param>
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.Created)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IActionResult> Add([FromBodyAttribute]QuestionIgnoreId question)
        {
            try
            { 
                if(question == null || string.IsNullOrEmpty(question.TopicId) )
                    return BadRequest();
                
                var topicId = question.TopicId;

                var topic = await _quizManager.GetTopicByIdAsync(topicId);
                if( topic == null )
                {
                    return NotFound("Topic not found");
                }

                await _quizManager.AddQuestionAsync(new ResponseData.Question {
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

        /// <summary>Delete Question</summary>
        /// <param name="id">Question Id</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if(string.IsNullOrEmpty(id))
                return BadRequest();
            try
            {
                await _quizManager.DeleteQuestionAsync(id);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"[Error ]{ex}");
                return BadRequest();
            }

            return NoContent();
        }

        /// <summary>Update Question</summary>
        /// <param name="id"> Question Id</param>
        /// <param name="question"> Question </param>
        [HttpPut("{id}")]
        [SwaggerResponse(HttpStatusCode.NoContent)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IActionResult> Update(string id, 
            [FromBodyAttribute] QuestionIgnoreId question )
        {
            if(string.IsNullOrEmpty(id) || question == null)
                return BadRequest();

            var result = await _quizManager.UpdateQuestionAsync(id, question);
            if(result != null)
                return NoContent();
            
            return BadRequest();
        }
        
        /// <summary> Update Question Properties </summary>
        /// <param name="questionId"> Existing Question Id </param>
        /// <param name="questions"> JSON Patch - rfc6902 </param>
        /// <response code="200">Returns updated item</response>
        /// <remarks>
        /// Example - Replace Topic id for an existing Question. 
        /// Note - Topic id should be an exisint topic id.
        ///
        /// PATCH /Questions
        /// [
        ///   {
        ///     "op": "replace",
        ///     "value": "59046a901b9fbd0016f2f04c",
        ///     "path": "/topicId"
        ///   }
        /// ]
        /// </remarks>
        [HttpPatch("{questionId}")]
        [ProducesResponseType(typeof(ResponseData.Question), 200)]
        public  async Task<IActionResult> UpdateTopic(string questionId, [FromBodyAttribute] 
                                    JsonPatchDocument<ResponseData.QuestionIgnoreId> questions)
        {                           // https://tools.ietf.org/html/rfc6902

            if(questions == null || string.IsNullOrEmpty(questionId))
                return BadRequest();
            
             if( string.IsNullOrEmpty(questionId) )
                return BadRequest("Question Id not valid");

            var updatedResult = await _quizManager.UpdateQuestionAsync(
                                questionId, questions);

            if( updatedResult != null)
                return Ok(updatedResult);

            return BadRequest();
        }
     }
 }