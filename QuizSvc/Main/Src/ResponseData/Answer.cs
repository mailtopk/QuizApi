using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ResponseData
{
    [JsonObjectAttribute]
    public class Answer
    {
        [JsonProperty("id")]
        public virtual string Id {get; set;}
        [JsonProperty("questionId")]
        [RequiredAttribute]
        public virtual string QuestionId {get; set;}
        [JsonProperty("description")]
        [RequiredAttribute]
        public string Description{get; set;}
        [JsonProperty("notes")]
        public string Notes{get; set;}
    }

    public class AnswerIgnoreId : Answer
    {
        [JsonIgnoreAttribute]
        public override string Id { get; set; }
    }

    public class AnswerIgnoreIdAndQuestion : AnswerIgnoreId
    {
        [JsonIgnoreAttribute]
        public override string QuestionId { get; set; }
    }
}