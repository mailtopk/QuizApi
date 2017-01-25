using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Answer
{
    [Route("api/quize/answer")]
    public class Answer : Controller 
    {

        [HttpGet]
        public Task<IActionResult> GetAll()
        {
            throw new NotImplementedException();
        }

        [HttpGet("{id}")]
        public Task<IActionResult> Get(string id)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public Task<IActionResult> Add([FromBodyAttribute]ResponseData.Answer answer)
        {
            throw new NotImplementedException();
        }
    }
}