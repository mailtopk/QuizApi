
using System.Threading.Tasks;
using TopicDataContract;

namespace TopicRepositoryLib
{
    public interface ITopicRepository 
    {
        Task<Topic> GetTopic(string id);
    }
    public class TopicRepository : ITopicRepository
    {
        public async Task<Topic> GetTopic(string id)
        {
            return await Task.Run( () => new Topic()).ConfigureAwait(false);
        }
    }
}
