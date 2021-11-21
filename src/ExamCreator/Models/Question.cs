using Newtonsoft.Json;

namespace ExamCreator.Models
{
    public class Question
    {
        [JsonProperty("query")]
        public string Query { get; set; }

        [JsonProperty("rightAnswer")]
        public int RightAnswer { get; set; }

        [JsonProperty("a")]
        public string A { get; set; }

        [JsonProperty("b")]
        public string B { get; set; }

        [JsonProperty("c")]
        public string C { get; set; }

        [JsonProperty("d")]
        public string D { get; set; }
    }
}
