using System.Linq;
using System.Threading.Tasks;
using QuizRepository;
using System;
using System.Collections.Generic;

namespace QuizManager
{
    public interface IQuizManager
    {
        // Get all results
        Task<IEnumerable<ResponseData.Topic>> GetAllTopicsAync();
        Task<IEnumerable<ResponseData.Question>> GetAllQuestionsAsync();
        Task<IEnumerable<ResponseData.Answer>> GetAllAnswersAsync();

        // Get by Id
        Task<ResponseData.Topic> GetTopicByIdAsync(string id);
        Task<ResponseData.Question> GetQuestionByIdAsync(string id);
        Task<ResponseData.Answer> GetAnswerByIdAsync(string id);
        Task<IEnumerable<ResponseData.Question>> GetQuestionByTopicIdAsync(string id);

        // Add 
        Task<string> AddTopicAsync(ResponseData.Topic topic);
        Task<string> AddQuestionAsync(ResponseData.Question question);
        Task<IEnumerable<ResponseData.Answer>> GetAnswersByQuestionIdAsync(string questionId);
        Task<string> AddAnswerAsync(ResponseData.Answer answer);
        
        
        // Update

        Task<ResponseData.Topic> UpdateTopicDescription(string topicId, string topicDescription);
        Task<ResponseData.Topic> UpdateTopic(string topicId, ResponseData.Topic topic);
        Task<ResponseData.Question> UpdateQuestionAsync(string questionId, ResponseData.Question question);
        Task<ResponseData.Question> PatchQuestion(string questionId, ResponseData.QuestionIgnoreId question);

        // Delete
        Task DeleteAnswer(string id);
        Task DeleteTopic(string id);

    }

    public class QuizManager : IQuizManager
    {
        private readonly ITopicRepository _topicRepository;
        private IQuestionRepository _questionRepository;
        private IAnswerRepository _answerRepository;

        public QuizManager(
            ITopicRepository topicRepository, 
            IQuestionRepository questionRepository,
            IAnswerRepository answerRepository)
        {
            _topicRepository = topicRepository;
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;

        }
        public async Task<IEnumerable<ResponseData.Answer>> GetAllAnswersAsync()
        {
            var results = await _answerRepository.GetAllAnswer();
            return results.Select( a => new ResponseData.Answer {
                Id = a.Id,
                QuestionId = a.QuestionId,
                Description = a.Description,
                Notes = a.Notes
            } );
        }

        public async Task<IEnumerable<ResponseData.Question>> GetAllQuestionsAsync()
        {
            var results = await _questionRepository.GetAllQuestionsAsync();
            return results.Select( q => new ResponseData.Question {
                Id = q.Id,
                TopicId = q.TopicId,
                Description = q.Description,
                Notes = q.Notes
            } );
        }

        public async Task<IEnumerable<ResponseData.Question>> GetQuestionByTopicIdAsync(string id)
        {
            try
            {
                var resultsQuestions = await _questionRepository.GetQuestionsByTopicAsync(id);
                return resultsQuestions.Select( q => new ResponseData.Question {
                    Id = q.Id,
                    TopicId = q.TopicId,
                    Description = q.Description,
                    Notes = q.Notes
                });
            }
            catch(Exception ex)
            {
                Console.WriteLine($"[Error ] : {ex}");
            }

            return null;
        }

        public async Task<IEnumerable<ResponseData.Topic>> GetAllTopicsAync()
        {
            try
            {
                var results = await _topicRepository.GetAllTopicsAsync();

                return results.Select ( t => new ResponseData.Topic  {
                    Id = t.Id,
                    Description = t.Description,
                    Notes = t.Notes
                }).ToList();
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine($"[Error ] {e.Message}");
                return null;
            }
        }

        public async Task<ResponseData.Answer> GetAnswerByIdAsync(string id)
        {
            var result = await _answerRepository.GetAnswer(id);
            return new ResponseData.Answer {
                Id = result.Id,
                QuestionId = result.QuestionId,
                Description = result.Description,
                Notes = result.Notes
            };
        }

        public async Task<ResponseData.Question> GetQuestionByIdAsync(string id)
        {
            var result = await _questionRepository.GetQuestionAsync(id);
            if( result != null)
            {
                return new ResponseData.Question {
                    Id = result.Id,
                    TopicId = result.TopicId,
                    Description = result.Description,
                    Notes = result.Notes
                };
            }

            return null;
        }

        public async Task<ResponseData.Topic> GetTopicByIdAsync(string id)
        {
            if(string.IsNullOrEmpty(id))
                return null;
                
            var result = await _topicRepository.GetTopicAsync(id);
            if(result != null)
            {
                return new ResponseData.Topic {
                    Id = result.Id,
                    Description = result.Description,
                    Notes = result.Notes
                };
            }
            
            return null;
        }

        public async Task<string> AddTopicAsync(ResponseData.Topic topic)
        {
            await _topicRepository.AddTopicAsync(new DataEntity.Topic {
                Description = topic.Description,
                Notes = topic.Notes
            });
            return ""; // TODO - return new topic id
        }

        public async Task<string> AddQuestionAsync(ResponseData.Question question)
        {
            if(question == null || question.TopicId == null)
                throw new ArgumentException("Invalid question or topicId");
                
            await _questionRepository.AddQuestionAsync( new DataEntity.Question {
                TopicId = question.TopicId,
                Description = question.Description,
                Notes = question.Notes
            } );
            return ""; //TODO - return new question id
        }

        public async Task<string> AddAnswerAsync(ResponseData.Answer answer)
        {
            await _answerRepository.AddAnswer( new DataEntity.Answer {
                QuestionId = answer.QuestionId,
                Description = answer.Description,
                Notes = answer.Notes
            } );

            return ""; // Fix this
        }

        public async Task<IEnumerable<ResponseData.Answer>> GetAnswersByQuestionIdAsync(string questionId)
        {
            var results = await _answerRepository.GetAnswerByQuestionId(questionId);
            if(results != null)
            {
                return results.Select( a => new ResponseData.Answer {
                    Id = a.Id,
                    QuestionId = a.QuestionId,
                    Description = a.Description,
                    Notes = a.Notes
                });
            }

            return null;
        }

        public async Task<ResponseData.Topic> UpdateTopicDescription(string topicId, string topicDescription)
        {
            var updatedEntity = await _topicRepository.UpdateDescriptionAsync(topicId, topicDescription);
            if( updatedEntity != null )
            {
                return new ResponseData.Topic {
                    Id = updatedEntity.Id,
                    Description = updatedEntity.Description,
                    Notes = updatedEntity.Notes
                };
            }

            return null;
        }

        public async Task<ResponseData.Topic> UpdateTopic(string topicId, ResponseData.Topic topic)
        {
            var resultUpdatedTopicEntity = await _topicRepository.UpdateTopicAsync (topicId, 
                                new DataEntity.Topic {
                                    Description = topic.Description,
                                    Notes = topic.Notes
                                });

            if( resultUpdatedTopicEntity != null )
            {
                return new ResponseData.Topic {
                    Id = resultUpdatedTopicEntity.Id,
                    Description = resultUpdatedTopicEntity.Description,
                    Notes = resultUpdatedTopicEntity.Notes
                };
            }
            return null;
        }

        public async Task<ResponseData.Question> UpdateQuestionAsync(string questionId, ResponseData.Question question)
        {
            var existingQuestion = await _questionRepository.GetQuestionAsync(questionId);
            if( existingQuestion == null )
                throw new Exception("Questions not found");
            
            var topic = await _topicRepository.GetTopicAsync(question.TopicId);
            if(topic == null)
                throw new  Exception("Topic not found");

            var results = await _questionRepository.UpdateAsync( questionId, new DataEntity.Question {
                TopicId = question.TopicId,
                Description = question.Description,
                Notes = question.Notes
            } );

            if( results != null)
            {
                return new ResponseData.Question{
                    Id = questionId,
                    TopicId = question.TopicId,
                    Description = question.Description,
                    Notes = question.Notes
                };
            }

            return null;
        }

        public async Task<ResponseData.Question> PatchQuestion(string questionId, 
                                            ResponseData.QuestionIgnoreId question)
        {
            // TODO - update only the properties which are changed.
            var result = await _questionRepository.UpdateAsync(questionId, new DataEntity.Question {
                TopicId = question.TopicId,
                Description = question.Description,
                Notes = question.Notes
            });

            return new ResponseData.Question{
                Id = result?.Id,
                TopicId = result?.TopicId,
                Description = result?.Description,
                Notes = result?.Notes
            };
        }

        public async Task DeleteTopic(string id)
        {
            if(string.IsNullOrEmpty(id))
                throw new ArgumentException("Invalid topic id");
            
            await _topicRepository.DeleteAsync(id);
        }

        public async Task DeleteAnswer(string id)
        {
            if(string.IsNullOrEmpty(id))
                throw new ArgumentException("Invalid answer Id");

            await _answerRepository.Delete(id);
        }
    }


}
