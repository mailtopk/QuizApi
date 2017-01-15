using Newtonsoft.Json;

namespace Data.Topic
{
    [JsonObjectAttribute]
    public class    Topic
    {
        [JsonIgnoreAttribute]
        public string Id {get; set;}

        [JsonPropertyAttribute("Description")]
        public string Description{get; set;}
        [JsonPropertyAttribute("Notes")]
        public string Notes {get; set;}
    }
}