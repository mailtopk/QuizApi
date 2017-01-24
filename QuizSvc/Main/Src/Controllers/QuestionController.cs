 using System;
 using System.Threading.Tasks;
 using Microsoft.AspNetCore.Mvc;
// using ResponseData;
 namespace Question
 {
     [RouteAttribute("api/quize/question")]
     public  class QuestionController : Controller
     {
         [HttpGet]
         public async Task<IActionResult>  GetAll()
         {
             throw new NotImplementedException();
         } 

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string Id)
        {
            throw new NotImplementedException();
        }  

        [HttpPost]
        public async Task<IActionResult> Add([FromBodyAttribute]ResponseData.Question question)
        {
            throw new NotImplementedException();
        }
     }
 }