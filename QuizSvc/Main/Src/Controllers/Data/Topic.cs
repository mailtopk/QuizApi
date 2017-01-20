using Newtonsoft.Json;

namespace ResponseData
{
    [JsonObjectAttribute]
    public class Topic
    {
        [JsonPropertyAttribute("Id")]
        public virtual string Id {get; set;}

        [JsonPropertyAttribute("Description")]
        public string Description{get; set;}

        [JsonPropertyAttribute("Notes")]
        public string Notes {get; set;}
    }

    public class TopicForAddtion :  Topic
    {
        [JsonIgnoreAttribute]
        public override string Id { get; set; }
    }
}