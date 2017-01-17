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
            try
            {
                var results = await _topicRepository.GetAllTopicsAsync();

                var response = results.Select ( t => new  {
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
                var result = new Data.Topic.Topic();
                var topicAwaiter =  await _topicRepository.GetTopicAsync(id);

                if(topicAwaiter == null)
                {
                    Console.WriteLine("object is empty");
                    return new OkObjectResult(result);
                }
                result.Id = topicAwaiter.Id;
                result.Description = topicAwaiter.Description;
                result.Notes = topicAwaiter.Notes;

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
        public async Task<IActionResult> AddTopic( [FromBodyAttribute] Data.Topic.Topic topic )
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

            return new ObjectResult(HttpStatusCode.Created);
        }
    }
}