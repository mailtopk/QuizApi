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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var results= await _quizManager.GetAllTopics();
            if(results != null)
                return new OkObjectResult(results);

            return new OkResult();
        }

        [HttpGet("{id}")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetById(string id)
        {
            if(string.IsNullOrEmpty (id))
                return BadRequest();
            
            try
            {
                var results =  await _quizManager.GetTopicById(id);
                return new ObjectResult(results);
            }
            catch (System.Exception ex)
            {
                _loggerTopic.LogCritical($"GetById{ex}");
                return BadRequest();
            }
        }

        [HttpPut("{id}/{description}")]
        [SwaggerResponse(HttpStatusCode.NotModified)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.NoContent)]
        public async Task<IActionResult> Update(string id, string description )
        {
            if(string.IsNullOrEmpty(id) || string.IsNullOrEmpty(description))
                return BadRequest();

            var response = await _quizManager.UpdateTopicDescription(id, description);
            if(response > 0)
                return new StatusCodeResult((int)HttpStatusCode.NoContent);

            return new StatusCodeResult((int)HttpStatusCode.NotModified);
        }

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

        [HttpPost]
        [SwaggerResponseAttribute(HttpStatusCode.Created)]
        [SwaggerResponseAttribute(HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddTopic( [FromBodyAttribute] ResponseData.TopicIgnoreUniqId topic )
        {
            try
            {
                await _quizManager.AddTopic(topic);
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