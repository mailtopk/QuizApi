using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.SwaggerGen.Annotations;
using System.Net;
using QuizManager;
using System;
using Microsoft.Extensions.Logging;

namespace TopicController
{
    [RouteAttribute("api/quiz/topics")]
    public class TopicController : Controller
    {
        private readonly IQuizManager _quizManager;
        private readonly ILogger _loggerTopic;
        public TopicController(IQuizManager quizManager, ILogger<TopicController> logger)
        {
            _quizManager = quizManager;
            _loggerTopic = logger;
        }

        /// <summary>
        /// Get all Topics
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var results= await _quizManager.GetAllTopicsAync();
            if(results != null)
                return new OkObjectResult(results);

            return new OkResult();
        }

        /// <summary>
        /// Get Topic by topic unique id
        /// </summary>
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
                _loggerTopic.LogCritical($"GetById{ex}");
                return BadRequest();
            }
        }

        /// <summary>
        /// Update existing Topic
        /// </summary>
        [HttpPut("{id}")]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.NoContent)]
        public async Task<IActionResult> Update(string id, 
                        [FromBodyAttribute] ResponseData.TopicIgnoreUniqId topic)
        {
             if( topic == null || string.IsNullOrEmpty(id) || 
                            string.IsNullOrEmpty(topic.Description) || 
                            string.IsNullOrEmpty(topic.Notes))
                {
                    return BadRequest();
                }

            var response = await _quizManager.UpdateTopic(id, topic);
            if(response != null)
                return new StatusCodeResult((int)HttpStatusCode.NoContent);

            return new StatusCodeResult((int)HttpStatusCode.NotModified);
        }

        /// <summary>
        /// Update Description of an existing Topic
        /// </summary>
        [HttpPatch("{id}/{description}")]
        [SwaggerResponse(HttpStatusCode.NotModified)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.NoContent)]
        public async Task<IActionResult> UpdateDescription(string id, string description )
        {
            if(string.IsNullOrEmpty(id) || string.IsNullOrEmpty(description))
                return BadRequest();

            var response = await _quizManager.UpdateTopicDescription(id, description);
            if(response != null)
                return new StatusCodeResult((int)HttpStatusCode.NoContent);

            return new StatusCodeResult((int)HttpStatusCode.NotModified);
        }

        /// <summary>
        /// Delete an existing Topic
        /// </summary>
        [HttpDelete("{id}")]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.NoContent)]
        public async Task<IActionResult> Delete(string id)
        {
            if(string.IsNullOrEmpty(id))
                return BadRequest();

            await _quizManager.DeleteTopic(id);
            return NoContent();
        }

        /// <summary>
        /// Create new Topic
        /// </summary>
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