using Newtonsoft.Json;
namespace ResponseData
{
    [JsonObjectAttribute]
    public class Question
    {
        [JsonPropertyAttribute("id")]
        public string Id {get; set;}
        [JsonPropertyAttribute("topicId")]
        public string TopicId {get; set;}
        [JsonPropertyAttribute("description")]
        public string Description{get; set;}
        [JsonPropertyAttribute("notes")]
        public string Notes {get; set;}
    }
}