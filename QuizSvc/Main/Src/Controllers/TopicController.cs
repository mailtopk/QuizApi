using System.Collections.Generic;
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
        public Task<IActionResult> GetAll()
        {
           throw new NotImplementedException();
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
        
    }
    
}