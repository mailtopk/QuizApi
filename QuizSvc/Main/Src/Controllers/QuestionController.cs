using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuizRepository;
using Swashbuckle.SwaggerGen.Annotations;

namespace Question
 {
     [RouteAttribute("api/quize/question")]
     public  class QuestionController : Controller
     {
         private IQuestionRepository _questionRepository;
         public QuestionController(IQuestionRepository questionRepository)
         {
             _questionRepository = questionRepository;
         }

         [HttpGet]

         [SwaggerResponse(HttpStatusCode.BadRequest)]
         public async Task<IActionResult>  GetAll()
         {
             try
             {
                var results = await _questionRepository.GetAllQuestionsAsync();
                var response = results.Select( q => new ResponseData.Question {
                    Id = q.Id,
                    TopicId = q.TopicId,
                    Description = q.Description,
                    Notes = q.Notes
                } );

                return new OkObjectResult(response);
             }
             catch(Exception ex)
             {
                 Console.WriteLine($"[Error ] {ex}");
                 return BadRequest();
             }
         } 

        [HttpGet("{id}")]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _questionRepository.GetQuestionAsync(id);
            
            if(result != null)
            {
                var response = new ResponseData.Question {
                        Id = result.Id,
                        TopicId = result.TopicId,
                        Description = result.Description,
                        Notes = result.Notes
                    };
                return new OkObjectResult(response);
            }
            
            return NotFound();
        }  

        [HttpGet("{topicId}/questions")]
        public async Task<IActionResult> GetQuestions(string topicId)
        {
            try
            {
                var results = await _questionRepository.GetQuestionsByTopic(topicId);
                if(results.Any())
                {
                    var response = results.Select( q => new ResponseData.Question {
                        Id = q.Id,
                        TopicId = q.TopicId,
                        Description = q.Description,
                        Notes = q.Notes
                    } );
                    return new OkObjectResult(response);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"[Error ] : {ex}");
                return BadRequest();
            }
            return NotFound();
        }

        [HttpPost]
        [SwaggerResponseAttribute(HttpStatusCode.Created)]
        public async Task<IActionResult> Add([FromBodyAttribute]ResponseData.QuestionsForAddtion question)
        {
            try
            {
                await _questionRepository.AddQuestionAsync( new DataEntity.Question {
                        TopicId = question.TopicId,
                        Description = question.Description,
                        Notes = question.Notes
                });
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest();
            }
            
            return new StatusCodeResult((int)HttpStatusCode.Created);
        }
     }
 }