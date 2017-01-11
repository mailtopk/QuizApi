using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.SwaggerGen.Annotations;
using System.Net;
using TopicRepositoryLib;
using System;

namespace TopicController
{
    [Route("api/[Controller]")]
    public class TopicController : Controller
    {
        private readonly ITopicRepository _topicRepository;
        public TopicController(ITopicRepository topicRepository)
        {
            _topicRepository = topicRepository;
        }

        [HttpGet(Name="Get")]
        public async Task<IActionResult> GetAll()
        {
           var results = await _topicRepository.GetAllTopics();

           var response = results.Select ( t => new Data.Topic.Topic {
               Id = t.Id,
               Description = t.Description
           }).ToList();

            return new ObjectResult(response);
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
            
            var topicAwaiter =  await _topicRepository.GetTopic(id);
            var result = new Data.Topic.Topic {
                Id = topicAwaiter.Id,
                Description = topicAwaiter.Description
            };
            return new ObjectResult(result);
        }

        [HttpPost]
        [SwaggerResponseAttribute(HttpStatusCode.Created)]
        [SwaggerResponseAttribute(HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddTopic( [FromBodyAttribute] Data.Topic.Topic topic )
        {
            try
            {
                await _topicRepository.AddTopic(new TopicDataContract.Topic {
                    Id = topic.Id,
                    Description = topic.Description
                });
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(400); // TODO - not good changes it later
            }

            return new ObjectResult(HttpStatusCode.Created);
        }
    }
}