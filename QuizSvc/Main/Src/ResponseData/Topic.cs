using Newtonsoft.Json;

namespace ResponseData
{
    [JsonObjectAttribute]
    public class Topic
    {
        [JsonPropertyAttribute("id")]
        public virtual string Id {get; set;}

        [JsonPropertyAttribute("description")]
        public string Description{get; set;}

        [JsonPropertyAttribute("notes")]
        public string Notes {get; set;}
    }

    public class TopicIgnoreUniqId :  Topic
    {
        [JsonIgnoreAttribute]
        public override string Id { get; set; }
    }
}