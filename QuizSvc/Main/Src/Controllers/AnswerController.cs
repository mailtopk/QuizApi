using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Answer
{
    [Route("api/quize/answer")]
    public class Answer : Controller 
    {

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            throw new NotImplementedException();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBodyAttribute]ResponseData.Answer answer)
        {
            throw new NotImplementedException();
        }
    }
}