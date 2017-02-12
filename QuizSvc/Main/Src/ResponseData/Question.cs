using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ResponseData
{
    [JsonObjectAttribute]
    public class Question
    {
        [JsonPropertyAttribute("id")]
        public virtual string Id {get; set;}
        [JsonPropertyAttribute("topicId")]
        [Required]
        public virtual string TopicId {get; set;}
        [JsonPropertyAttribute("description")]
        [Required]
        public string Description{get; set;}
        [JsonPropertyAttribute("notes")]
        public string Notes {get; set;}
    }

    public class QuestionIgnoreId : Question
    {
        [JsonIgnoreAttribute]
        public override string Id { get; set; }
    }

    public class QuestionsIgnoreTopicIdAndQuestionId : QuestionIgnoreId
    {
        [JsonIgnoreAttribute]
        public override string TopicId { get; set; }
    }
}