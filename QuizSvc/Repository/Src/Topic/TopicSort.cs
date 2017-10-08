using System.Collections.Generic;
using System.Threading.Tasks;
using DataEntity;
using System.Linq;

namespace  QuizRepository.TopicSort {
    public abstract class TopicSort : ITopicRepository
    {
        protected ITopicRepository TopicRepository {get; set;}
        public TopicSort(ITopicRepository topicRepository)
        {
            TopicRepository = topicRepository;
        }
        public async Task DeleteAsync(string id) =>  await TopicRepository.DeleteAsync(id);

        public virtual async Task<IEnumerable<Topic>> GetAllTopicsAsync() => await TopicRepository.GetAllTopicsAsync();

        public async Task<Topic> GetTopicAsync(string id) => await TopicRepository.GetTopicAsync(id);

        public async Task<Topic> UpdateTopicAsync(string id, Topic topic) 
                                                    => await TopicRepository.UpdateTopicAsync(id, topic);
        public async Task AddTopicAsync(Topic topic) =>  await TopicRepository.AddTopicAsync(topic);
    }

    public class TopicSortByDescription : TopicSort
    {
        public TopicSortByDescription(TopicRepository topicRepository) 
        : base(topicRepository) {}

        public override async Task<IEnumerable<Topic>> GetAllTopicsAsync()
        {
            var topicResults = await TopicRepository.GetAllTopicsAsync();
            return topicResults.OrderBy( t => t.Description );
        }
    }

    public class TopicSortById : TopicSort
    {
        public TopicSortById(TopicRepository topicRepository) : 
            base (topicRepository)
        {
            
        }

        public override async Task<IEnumerable<Topic>> GetAllTopicsAsync()
        {
            var topicResults = await TopicRepository.GetAllTopicsAsync();
            return topicResults.OrderBy( t => t.Id );
        }
    }
}