using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.SwaggerGen.Annotations;
using System.Net;
using QuizManager;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.JsonPatch;

namespace TopicController
{
    [RouteAttribute("api/quiz/topics")]
    public class TopicController : Controller
    {
        private readonly IQuizManager _quizManager;
        private readonly ILogger<TopicController> _loggerTopic;
        public TopicController(IQuizManager quizManager, ILogger<TopicController> logger)
        {
            _quizManager = quizManager;
            _loggerTopic = logger;
        }

        /// <summary>Get all Topics</summary>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll()
        {
            var results= await _quizManager.GetAllTopicsAync();
            if(results != null)
                return new OkObjectResult(results);

            return NotFound();
        }

        /// <summary>Get Topic by topic unique id</summary>
        /// <param name="id"> Topic Id </param>
        [HttpGet("{id}")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetById(string id)
        {
            if(string.IsNullOrEmpty (id))
                return BadRequest();
            
            try
            {
                var results =  await _quizManager.GetTopicByIdAsync(id);
                return new ObjectResult(results);
            }
            catch (System.Exception ex)
            {
                 _loggerTopic.LogError($"Error {ex}");
                 return BadRequest();
            }
        }

        /// <summary> Update existing Topic </summary>
        /// <param name="id">Topic Id</param>
        /// <param name="topic">Topic</param>
        [HttpPut("{id}")]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.NoContent)]
        [SwaggerResponse(HttpStatusCode.NotModified)]
        public async Task<IActionResult> Update(string id, 
                        [FromBodyAttribute] ResponseData.TopicIgnoreUniqId topic)
        {
             if( topic == null || string.IsNullOrEmpty(id) || 
                            string.IsNullOrEmpty(topic.Description) || 
                            string.IsNullOrEmpty(topic.Notes))
                {
                    return BadRequest();
                }

            try
            {
                 var response = await _quizManager.UpdateTopic(id, topic);
                if(response != null)
                    return new StatusCodeResult((int)HttpStatusCode.NoContent);
            }
            catch(Exception ex)
            {
                _loggerTopic.LogCritical($"Error in topic Update : {ex}");
            }

            return new StatusCodeResult((int)HttpStatusCode.NotModified);
        }

        /// <summary>Update an existing Topic</summary>
        /// <param name="id"> Topic id </param>
        /// <param name="topic"> JSON Patch - rfc6902 </param>
        /// <remarks>
        /// Example - Replace Topic description and add notes for an existing Question. 
        ///
        /// PATCH /topics
        /// [
        ///   {
        ///     "op": "replace",
        ///     "path": "/description"
        ///     "value": "Description foo"
        ///   }
        ///   {
        ///     "op": "add",
        ///     "path": "/notes"
        ///     "value": "new foo notes"
        ///   }
        /// ]
        ///
        /// </remarks>
        [HttpPatch("{id}")]
        [SwaggerResponse(HttpStatusCode.NotModified)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateTopic(string id, 
                            [FromBodyAttribute] JsonPatchDocument<ResponseData.TopicIgnoreUniqId> topic )
        {
            if(topic == null || string.IsNullOrEmpty(id))
                return BadRequest();
            try
            {
                var updatedTopic = await _quizManager.UpdateTopicAsync(id, topic);
                if(updatedTopic != null)
                    return Ok(updatedTopic);
            }
            catch(Exception ex)
            {
                _loggerTopic.LogCritical($"AddTopic{ex}");
            }

            return new StatusCodeResult((int)HttpStatusCode.NotModified);
        }


        /// <summary> Delete an existing Topic </summary>
        /// <param name="id"> Topic id </param>
        [HttpDelete("{id}")]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.NoContent)]
        public async Task<IActionResult> Delete(string id)
        {
            if(string.IsNullOrEmpty(id))
                return BadRequest();

            await _quizManager.DeleteTopicAsync(id);
            return NoContent();
        }

        /// <summary>Create new Topic</summary>
        /// <param name="topic">Topic</param>
        [HttpPost]
        [SwaggerResponseAttribute(HttpStatusCode.Created)]
        [SwaggerResponseAttribute(HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddTopic( [FromBodyAttribute] ResponseData.TopicIgnoreUniqId topic )
        {
            try
            {
                await _quizManager.AddTopicAsync(topic);
            }
            catch(Exception ex)
            {
                _loggerTopic.LogCritical($"AddTopic{ex}");
                return BadRequest();
            }
            
           // Response.Headers.Add("Location", ""); // TODO send new objectid 
            return new StatusCodeResult((int)HttpStatusCode.Created);
        }
    }
}