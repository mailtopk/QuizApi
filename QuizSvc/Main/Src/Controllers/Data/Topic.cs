using Newtonsoft.Json;

namespace Data.Topic
{
    [JsonObjectAttribute]
    public class Topic
    {
        [JsonPropertyAttribute("id")]
        public string Id {get; set;}
        [JsonPropertyAttribute("Description")]
        public string Description{get; set;}
    }
}