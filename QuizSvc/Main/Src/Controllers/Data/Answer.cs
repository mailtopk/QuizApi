using Newtonsoft.Json;

namespace ResponseData
{
    [JsonObjectAttribute]
    public class Answer
    {
        [JsonProperty("id")]
        public string Id {get; set;}
        [JsonProperty("questionId")]
        public string QuestionId {get; set;}
        [JsonProperty("description")]
        public string Description{get; set;}
        [JsonProperty("notes")]
        public string Notes{get; set;}

    }
}