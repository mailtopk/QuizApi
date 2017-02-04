using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.SwaggerGen.Annotations;
using System.Net;
using QuizRepository;
using System;

namespace TopicController
{
    [RouteAttribute("api/quiz/topic")]
    public class TopicController : Controller
    {
        private readonly ITopicRepository _topicRepository;
        public TopicController(ITopicRepository topicRepository)
        {
            _topicRepository = topicRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var results = await _topicRepository.GetAllTopicsAsync();

                var response = results.Select ( t => new ResponseData.Topic  {
                    Id = t.Id,
                    Description = t.Description,
                    Notes = t.Notes
                }).ToList();

                return new ObjectResult(response);
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.Message);
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetById(string id)
        {
            if(string.IsNullOrEmpty (id))
            {
                return BadRequest();
            }
            
            try
            {
                var topicAwaiter =  await _topicRepository.GetTopicAsync(id);
                
                if(topicAwaiter == null)
                {
                    return NoContent();
                }

                var result = new ResponseData.Topic {
                    Id = topicAwaiter.Id,
                    Description = topicAwaiter.Description,
                    Notes = topicAwaiter.Notes
                };

                return new ObjectResult(result);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest();
            }
        }

        [HttpPost]
        [SwaggerResponseAttribute(HttpStatusCode.Created)]
        [SwaggerResponseAttribute(HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddTopic( [FromBodyAttribute] ResponseData.TopicForAddtion topic )
        {
            try
            {
                await _topicRepository.AddTopicAsync(new DataEntity.Topic {
                    Description = topic.Description,
                    Notes = topic.Notes
                });
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest();
            }
            
           // Response.Headers.Add("Location", ""); // TODO send new objectid 
            return new StatusCodeResult((int)HttpStatusCode.Created);
        }
    }
}