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

         /// <summary>
        /// Get all Question's
        /// </summary>
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

         /// <summary>
        /// Get Question by question unique id
        /// </summary>
        [HttpGet("{id}")]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get(string id)
        {
            var response = await _quizManager.GetQuestionByIdAsync(id);
            if(response != null)
                return new OkObjectResult(response);

            return NotFound();
        }  

        /// <summary>
        /// Get Questions for a given topic
        /// </summary>
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

        /// <summary>
        /// Create new Question
        /// </summary>
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

        /// <summary>
        /// Delete Question
        /// </summary>
        [HttpDelete("{id}")]
        public Task<IActionResult> Delete(string id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Update Question
        /// </summary>
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

      
        /// <summary>
        /// Update Question Properties
        /// </summary>
        [HttpPatch("{questionId}")]
        public  async Task<IActionResult> UpdateTopic(string questionId, [FromBodyAttribute] 
                                    JsonPatchDocument<ResponseData.QuestionIgnoreId> questions)
        {                           // https://tools.ietf.org/html/rfc6902

            if(questions == null || string.IsNullOrEmpty(questionId))
                return BadRequest();
            
             if( string.IsNullOrEmpty(questionId) )
                return BadRequest("Question Id not valid");

            // TODO - SRP is broken - fix this
            var existingQuestion = await _quizManager.GetQuestionByIdAsync(questionId);
            if(existingQuestion == null || questions == null)
                return NotFound();

            // TODO - automapper ?
            var questionUpdateRequest = new ResponseData.QuestionIgnoreId{
                Description = existingQuestion.Description,
                Notes = existingQuestion.Notes,
                TopicId = existingQuestion.TopicId
            };

            if(questionUpdateRequest != null)
                questions.ApplyTo(questionUpdateRequest );
            

            var updatedResult = await _quizManager.PatchQuestion(
                                questionId, questionUpdateRequest);

            if( updatedResult != null)
                return Ok(updatedResult);

            return BadRequest();
        }
     }
 }